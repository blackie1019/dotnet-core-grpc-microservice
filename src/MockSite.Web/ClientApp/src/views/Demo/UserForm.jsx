
import React from "react";
import axios from 'axios'

import SweetAlert from "react-bootstrap-sweetalert";

// @material-ui/core components
import withStyles from "@material-ui/core/styles/withStyles";
// @material-ui/icons
import MailOutline from "@material-ui/icons/MailOutline";

// core components
import GridContainer from "components/Grid/GridContainer.jsx";
import GridItem from "components/Grid/GridItem.jsx";
import CustomInput from "components/CustomInput/CustomInput.jsx";
import Button from "components/CustomButtons/Button.jsx";
import Card from "components/Card/Card.jsx";
import CardHeader from "components/Card/CardHeader.jsx";
import CardIcon from "components/Card/CardIcon.jsx";
import CardBody from "components/Card/CardBody.jsx";

import regularFormsStyle from "assets/jss/material-dashboard-pro-react/views/regularFormsStyle";

function convertToInt(value) {
  var parsed = parseInt(value, 10);
  if (isNaN(parsed)) { return 0 }
  return parsed;
}

class UserForm extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      alert: null,
      show: false
    };
    this.hideAlert = this.hideAlert.bind(this);
    this.successAlert = this.successAlert.bind(this);

    this.handleChangeCode = this.handleChangeCode.bind(this);
    this.handleChangeDisplayKey = this.handleChangeDisplayKey.bind(this);
    this.handleChangeOrderNo = this.handleChangeOrderNo.bind(this);

    this.handleSubmit = this.handleSubmit.bind(this);
  }

  componentDidMount() {
      if (localStorage.getItem("token") === "") {
          this.props.history.push('/authorization/login');
      }
  }
  
  handleChangeCode(event) {
    this.setState({ Code: event.target.value });
  }

  handleChangeDisplayKey(event) {
    this.setState({ DisplayKey: event.target.value });
  }

  handleChangeOrderNo(event) {
    this.setState({ OrderNo: event.target.value });
  }

  successAlert() {
    this.setState({
      alert: (
        <SweetAlert
          success
          style={{ display: "block", marginTop: "-100px" }}
          title="Good job!"
          onConfirm={() => this.hideAlert(true)}
          onCancel={() => this.hideAlert(false)}
          confirmBtnCssClass={
            this.props.classes.button + " " + this.props.classes.success
          }
        >
          You clicked the button!
        </SweetAlert>
      )
    });
  }

  handleSubmit (event) {
    event.preventDefault();
    
    const user = {
      code:convertToInt(this.state.Code),
      displayKey:this.state.DisplayKey,
      orderNo:convertToInt(this.state.OrderNo)
    };
    
    axios.post(`https://localhost:5001/api/User/CreateUser`,  user )
    .then(res => {
      if(res.data.code ===0){
        this.successAlert();
      }
    })
  }

  hideAlert(isReturnList) {
    this.setState({
      alert: null
    });
    if(isReturnList){
      this.props.history.push('/user/list'); 
    }
  }

  render() {
    const { classes } = this.props;
    return (
      <div>
        {this.state.alert}
        <GridContainer>
          <GridItem xs={12} sm={12} md={12}>
            <Card>
              <CardHeader color="rose" icon>
                <CardIcon color="rose">
                  <MailOutline />
                </CardIcon>
                <h4 className={classes.cardIconTitle}>User Form</h4>
              </CardHeader>
              <CardBody>
                <form onSubmit={this.handleSubmit}>
                  <CustomInput
                    labelText="Code"
                    id="code"
                    formControlProps={{
                      fullWidth: true
                    }}
                    inputProps={{
                      onChange : this.handleChangeCode,
                      type: "code"
                    }}
                  />
                  <CustomInput
                    labelText="Display Key"
                    id="displaykey"
                    formControlProps={{
                      fullWidth: true
                    }}
                    inputProps={{
                      onChange : this.handleChangeDisplayKey,
                      type: "displaykey"
                    }}
                  />
                  <CustomInput
                    labelText="OrderNo"
                    id="orderno"
                    formControlProps={{
                      fullWidth: true
                    }}
                    inputProps={{
                      onChange : this.handleChangeOrderNo,
                      type: "orderno"
                    }}
                  />
                  <Button color="rose" type="submit">Submit</Button>
                </form>
              </CardBody>
            </Card>
          </GridItem>
        </GridContainer>
      </div>
    );
  }
}

export default withStyles(regularFormsStyle)(UserForm);
