import React from "react";

// @material-ui/core components
import withStyles from "@material-ui/core/styles/withStyles";

// material-ui icons
import Assignment from "@material-ui/icons/Assignment";

// core components
import GridContainer from "components/Grid/GridContainer.jsx";
import GridItem from "components/Grid/GridItem.jsx";
import Table from "components/Table/Table.jsx";
import Card from "components/Card/Card.jsx";
import CardHeader from "components/Card/CardHeader.jsx";
import CardIcon from "components/Card/CardIcon.jsx";
import CardBody from "components/Card/CardBody.jsx";

import { cardTitle } from "assets/jss/material-dashboard-pro-react.jsx";

const style = {
  customCardContentClass: {
    paddingLeft: "0",
    paddingRight: "0"
  },
  cardIconTitle: {
    ...cardTitle,
    marginTop: "15px",
    marginBottom: "0px"
  }
};

function RegularTables({ ...props }) {
  const { classes } = props;
  return (
    <GridContainer>
      <GridItem xs={12}>
        <Card>
          <CardHeader color="rose" icon>
            <CardIcon color="rose">
              <Assignment />
            </CardIcon>
            <h4 className={classes.cardIconTitle}>Simple Table</h4>
          </CardHeader>
          <CardBody>
            <Table
              tableHeaderColor="primary"
              tableHead={["Name", "Country", "City", "Salary"]}
              tableData={[
                ["Dakota Rice", "Niger", "Oud-Turnhout", "$36,738"],
                ["Minerva Hooper", "Curaçao", "Sinaai-Waas", "$23,789"],
                ["Sage Rodriguez", "Netherlands", "Baileux", "$56,142"],
                ["Philip Chaney", "Korea, South", "Overland Park", "$38,735"],
                ["Doris Greene", "Malawi", "Feldkirchen in Kärnten", "$63,542"],
                ["Mason Porter", "Chile", "Gloucester", "$78,615"]
              ]}
              coloredColls={[3]}
              colorsColls={["primary"]}
            />
          </CardBody>
        </Card>
      </GridItem>
      <GridItem xs={12}>
        <Card plain>
          <CardHeader color="rose" icon plain>
            <CardIcon color="rose">
              <Assignment />
            </CardIcon>
            <h4 className={classes.cardIconTitle}>
              Table on Plain Background<small>
                {" "}
                - Here is a subtitle for this table
              </small>
            </h4>
          </CardHeader>
          <CardBody plain>
            <Table
              hover
              tableHead={["ID", "Name", "Salary", "Country", "City"]}
              tableData={[
                ["1", "Dakota Rice", "$36,738", "Niger", "Oud-Turnhout"],
                ["2", "Minerva Hooper", "$23,789", "Curaçao", "Sinaai-Waas"],
                ["3", "Sage Rodriguez", "$56,142", "Netherlands", "Baileux"],
                [
                  "4",
                  "Philip Chaney",
                  "$38,735",
                  "Korea, South",
                  "Overland Park"
                ],
                [
                  "5",
                  "Doris Greene",
                  "$63,542",
                  "Malawi",
                  "Feldkirchen in Kärnten"
                ],
                ["6", "Mason Porter", "$78,615", "Chile", "Gloucester"]
              ]}
            />
          </CardBody>
        </Card>
      </GridItem>
      <GridItem xs={12}>
        <Card>
          <CardHeader color="rose" icon>
            <CardIcon color="rose">
              <Assignment />
            </CardIcon>
            <h4 className={classes.cardIconTitle}>Regular Table with Colors</h4>
          </CardHeader>
          <CardBody className={classes.customCardContentClass}>
            <Table
              hover
              tableHead={["ID", "Name", "Salary", "Country", "City"]}
              tableData={[
                {
                  color: "success",
                  data: [
                    "1",
                    "Dakota Rice (Success)",
                    "$36,738",
                    "Niger",
                    "Oud-Turnhout"
                  ]
                },
                ["2", "Minerva Hooper", "$23,789", "Curaçao", "Sinaai-Waas"],
                {
                  color: "info",
                  data: [
                    "3",
                    "Sage Rodriguez (Info)",
                    "$56,142",
                    "Netherlands",
                    "Baileux"
                  ]
                },
                [
                  "4",
                  "Philip Chaney",
                  "$38,735",
                  "Korea, South",
                  "Overland Park"
                ],
                {
                  color: "danger",
                  data: [
                    "5",
                    "Doris Greene (Danger)",
                    "$63,542",
                    "Malawi",
                    "Feldkirchen in Kärnten"
                  ]
                },
                ["6", "Mason Porter", "$78,615", "Chile", "Gloucester"],
                {
                  color: "warning",
                  data: [
                    "7",
                    "Mike Chaney (Warning)",
                    "$38,735",
                    "Romania",
                    "Bucharest"
                  ]
                }
              ]}
            />
          </CardBody>
        </Card>
      </GridItem>
    </GridContainer>
  );
}

export default withStyles(style)(RegularTables);
