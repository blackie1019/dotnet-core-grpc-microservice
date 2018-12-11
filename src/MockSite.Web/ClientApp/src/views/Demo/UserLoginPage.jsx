import React from "react";
import PropTypes from "prop-types";

// @material-ui/core components
import withStyles from "@material-ui/core/styles/withStyles";
import InputAdornment from "@material-ui/core/InputAdornment";
import Icon from "@material-ui/core/Icon";

import SweetAlert from "react-bootstrap-sweetalert";

// @material-ui/icons
import Face from "@material-ui/icons/Face";
import Email from "@material-ui/icons/Email";
// import LockOutline from "@material-ui/icons/LockOutline";

// core components
import GridContainer from "components/Grid/GridContainer.jsx";
import GridItem from "components/Grid/GridItem.jsx";
import CustomInput from "components/CustomInput/CustomInput.jsx";
import Button from "components/CustomButtons/Button.jsx";
import Card from "components/Card/Card.jsx";
import CardBody from "components/Card/CardBody.jsx";
import CardHeader from "components/Card/CardHeader.jsx";
import CardFooter from "components/Card/CardFooter.jsx";

import loginPageStyle from "assets/jss/material-dashboard-pro-react/views/loginPageStyle.jsx";
import axios from "axios";

class UserLoginPage extends React.Component {
  constructor(props) {
    super(props);
    localStorage.removeItem("token");
    // we use this to make the card to appear after the page has been rendered
    this.state = {
        cardAnimaton: "cardHidden",
        alert: null,
        show: false
    };

    this.hideAlert = this.hideAlert.bind(this);
    this.successAlert = this.successAlert.bind(this);

    this.handleChangeUserName = this.handleChangeUserName.bind(this);
    this.handleChangePassword = this.handleChangePassword.bind(this);

    this.handleSubmit = this.handleSubmit.bind(this);
  }
  
  componentDidMount() {
    // we add a hidden class to the card and after 700 ms we delete it and the transition appears
    this.timeOutFunction = setTimeout(
      function() {
        this.setState({ cardAnimaton: "" });
      }.bind(this),
      700
    );
  }
  
  componentWillUnmount() {
    clearTimeout(this.timeOutFunction);
    this.timeOutFunction = null;
  }

  handleChangeUserName(event) {
      this.setState({ UserName: event.target.value });
  }

  handleChangePassword(event) {
      this.setState({ Password: event.target.value });
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
      
      const loginUser = {
          username: this.state.UserName,
          password: this.state.Password
      };
      
      axios.post('https://localhost:5001/api/Authorized/login', loginUser)
          .then(response => {
              localStorage.setItem("token", `Bearer ${response.data.token}`);
              this.successAlert();
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
      <div className={classes.container}>
          {this.state.alert}
        <GridContainer justify="center">
          <GridItem xs={12} sm={6} md={4}>
            <form onSubmit={this.handleSubmit}>
              <Card login className={classes[this.state.cardAnimaton]}>
                <CardHeader
                  className={`${classes.cardHeader} ${classes.textCenter}`}
                  color="rose"
                >
                  <h4 className={classes.cardTitle}>Log in</h4>
                </CardHeader>
                <CardBody>
                  <CustomInput
                    labelText="User Name.."
                    id="username"
                    formControlProps={{
                      fullWidth: true
                    }}
                    inputProps={{
                      onChange : this.handleChangeUserName,
                      endAdornment: (
                        <InputAdornment position="end">
                          <Face className={classes.inputAdornmentIcon} />
                        </InputAdornment>
                      )
                    }}
                  />
                  <CustomInput
                    labelText="Password"
                    id="password"
                    formControlProps={{
                      fullWidth: true
                    }}
                    inputProps={{
                      onChange : this.handleChangePassword,
                      endAdornment: (
                        <InputAdornment position="end">
                          <Icon className={classes.inputAdornmentIcon}>
                            lock_outline
                          </Icon>
                        </InputAdornment>
                      )
                    }}
                  />
                </CardBody>
                <CardFooter className={classes.justifyContentCenter}>
                  <Button type="submit" color="rose" simple size="lg" block>
                    Let's Go
                  </Button>
                </CardFooter>
              </Card>
            </form>
          </GridItem>
        </GridContainer>
      </div>
    );
  }
}

UserLoginPage.propTypes = {
  classes: PropTypes.object.isRequired
};

export default withStyles(loginPageStyle)(UserLoginPage);
