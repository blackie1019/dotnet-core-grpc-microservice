import React from "react";
import PropTypes from "prop-types";

// @material-ui/core components
import withStyles from "@material-ui/core/styles/withStyles";
import Icon from "@material-ui/core/Icon";

// @material-ui/icons
// import Weekend from "@material-ui/icons/Weekend";
import Home from "@material-ui/icons/Home";
import Business from "@material-ui/icons/Business";
import AccountBalance from "@material-ui/icons/AccountBalance";

// core components
import GridContainer from "components/Grid/GridContainer.jsx";
import GridItem from "components/Grid/GridItem.jsx";
import Button from "components/CustomButtons/Button.jsx";
import Card from "components/Card/Card.jsx";
import CardBody from "components/Card/CardBody.jsx";

import pricingPageStyle from "assets/jss/material-dashboard-pro-react/views/pricingPageStyle.jsx";

class PricingPage extends React.Component {
  render() {
    const { classes } = this.props;
    return (
      <div className={classes.container}>
        <GridContainer justify="center">
          <GridItem xs={12} sm={12} md={6}>
            <h2 className={classes.title}>Pick the best plan for you</h2>
            <h5 className={classes.description}>
              You have Free Unlimited Updates and Premium Support on each
              package.
            </h5>
          </GridItem>
        </GridContainer>
        <GridContainer justify="center">
          <GridItem xs={12} sm={12} md={3}>
            <Card pricing plain>
              <CardBody pricing plain>
                <h6 className={classes.cardCategory}>Freelancer</h6>
                <div className={classes.icon}>
                  <Icon className={classes.iconWhite}>weekend</Icon>
                </div>
                <h3
                  className={`${classes.cardTitleWhite} ${classes.marginTop30}`}
                >
                  FREE
                </h3>
                <p className={classes.cardCategory}>
                  This is good if your company size is between 2 and 10 Persons.
                </p>
                <Button round color="white">
                  Choose plan
                </Button>
              </CardBody>
            </Card>
          </GridItem>
          <GridItem xs={12} sm={12} md={3}>
            <Card pricing raised>
              <CardBody pricing>
                <h6 className={classes.cardCategory}>SMALL COMPANY</h6>
                <div className={classes.icon}>
                  <Home className={classes.iconRose} />
                </div>
                <h3 className={`${classes.cardTitle} ${classes.marginTop30}`}>
                  $29
                </h3>
                <p className={classes.cardDescription}>
                  This is good if your company size is between 2 and 10 Persons.
                </p>
                <Button round color="rose">
                  Choose plan
                </Button>
              </CardBody>
            </Card>
          </GridItem>
          <GridItem xs={12} sm={12} md={3}>
            <Card pricing plain>
              <CardBody pricing plain>
                <h6 className={classes.cardCategory}>MEDIUM COMPANY</h6>
                <div className={classes.icon}>
                  <Business className={classes.iconWhite} />
                </div>
                <h3
                  className={`${classes.cardTitleWhite} ${classes.marginTop30}`}
                >
                  $69
                </h3>
                <p className={classes.cardCategory}>
                  This is good if your company size is between 11 and 99
                  Persons.
                </p>
                <Button round color="white">
                  Choose plan
                </Button>
              </CardBody>
            </Card>
          </GridItem>
          <GridItem xs={12} sm={12} md={3}>
            <Card pricing plain>
              <CardBody pricing plain>
                <h6 className={classes.cardCategory}>ENTERPRISE</h6>
                <div className={classes.icon}>
                  <AccountBalance className={classes.iconWhite} />
                </div>
                <h3
                  className={`${classes.cardTitleWhite} ${classes.marginTop30}`}
                >
                  $159
                </h3>
                <p className={classes.cardCategory}>
                  This is good if your company size is 99+ persons.
                </p>
                <Button round color="white">
                  Choose plan
                </Button>
              </CardBody>
            </Card>
          </GridItem>
        </GridContainer>
      </div>
    );
  }
}

PricingPage.propTypes = {
  classes: PropTypes.object.isRequired
};

export default withStyles(pricingPageStyle)(PricingPage);
