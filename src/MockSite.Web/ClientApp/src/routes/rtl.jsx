import Dashboard from "views/Dashboard/Dashboard.jsx";
import Buttons from "views/Components/Buttons.jsx";
import GridSystem from "views/Components/GridSystem.jsx";
import Panels from "views/Components/Panels.jsx";
import SweetAlert from "views/Components/SweetAlert.jsx";
import Notifications from "views/Components/Notifications.jsx";
import Icons from "views/Components/Icons.jsx";
import Typography from "views/Components/Typography.jsx";
import RegularForms from "views/Forms/RegularForms.jsx";
import ExtendedForms from "views/Forms/ExtendedForms.jsx";
import ValidationForms from "views/Forms/ValidationForms.jsx";
import Wizard from "views/Forms/Wizard.jsx";
import RegularTables from "views/Tables/RegularTables.jsx";
import ExtendedTables from "views/Tables/ExtendedTables.jsx";
import ReactTables from "views/Tables/ReactTables.jsx";
import GoogleMaps from "views/Maps/GoogleMaps.jsx";
import FullScreenMap from "views/Maps/FullScreenMap.jsx";
import VectorMap from "views/Maps/VectorMap.jsx";
import Charts from "views/Charts/Charts.jsx";
import Calendar from "views/Calendar/Calendar.jsx";
import Widgets from "views/Widgets/Widgets.jsx";
import UserProfile from "views/Pages/UserProfile.jsx";
import TimelinePage from "views/Pages/Timeline.jsx";
import RTLSupport from "views/Pages/RTLSupport.jsx";
import PricingPage from "views/Pages/PricingPage.jsx";
import LoginPage from "views/Pages/LoginPage.jsx";
import RegisterPage from "views/Pages/RegisterPage.jsx";
import LockScreenPage from "views/Pages/LockScreenPage.jsx";

// @material-ui/icons
import DashboardIcon from "@material-ui/icons/Dashboard";
import Image from "@material-ui/icons/Image";
import Apps from "@material-ui/icons/Apps";
// import ContentPaste from "@material-ui/icons/ContentPaste";
import GridOn from "@material-ui/icons/GridOn";
import Place from "@material-ui/icons/Place";
import WidgetsIcon from "@material-ui/icons/Widgets";
import Timeline from "@material-ui/icons/Timeline";
import DateRange from "@material-ui/icons/DateRange";
import PersonAdd from "@material-ui/icons/PersonAdd";
import Fingerprint from "@material-ui/icons/Fingerprint";
import MonetizationOn from "@material-ui/icons/MonetizationOn";
import LockOpen from "@material-ui/icons/LockOpen";

const rtlRoutes = [
  {
    path: "/rtl/rtl-support-page",
    name: "دعم رتل",
    mini: "ص",
    component: RTLSupport
  },
  {
    redirect: true,
    path: "/rtl",
    pathTo: "/rtl/rtl-support-page",
    name: "RTL Support"
  }
];

var sidebarLinks = [
  {
    path: "/dashboard",
    name: "لوحة القيادة ة",
    icon: DashboardIcon,
    component: Dashboard
  },
  {
    collapse: true,
    path: "-page",
    name: "صفحات",
    state: "openPages",
    icon: Image,
    views: [
      {
        path: "/timeline-page",
        name: "الجدول الزمني",
        mini: "ر",
        component: TimelinePage
      },
      {
        path: "/user-page",
        name: "ملف تعريفي للمستخدم",
        mini: "شع",
        component: UserProfile
      },
      {
        path: "/rtl/rtl-support-page",
        name: "دعم رتل",
        mini: "ص",
        component: RTLSupport
      },
      {
        path: "/pages/register-page",
        name: "سجل الصفحة",
        short: "سجل الصفحة",
        mini: "صع",
        icon: PersonAdd,
        component: RegisterPage
      },
      {
        path: "/pages/login-page",
        name: "صفحة تسجيل الدخول",
        short: "صفحة تسجيل الدخول",
        mini: "هعذا",
        icon: Fingerprint,
        component: LoginPage
      },
      {
        path: "/pages/pricing-page",
        name: "التسعير",
        short: "التسعير",
        mini: "ع",
        icon: MonetizationOn,
        component: PricingPage
      },
      {
        path: "/pages/lock-screen-page",
        name: "قفل صفحة الشاشة",
        short: "قفل صفحة الشاشة",
        mini: "هذاع",
        icon: LockOpen,
        component: LockScreenPage
      },
      {
        redirect: true,
        path: "/pages",
        pathTo: "/pages/register-page",
        name: "Register Page"
      }
    ]
  },
  {
    collapse: true,
    path: "/components",
    name: "المكونات",
    state: "openComponents",
    icon: Apps,
    views: [
      {
        path: "/components/buttons",
        name: "وصفتت",
        mini: "بت",
        component: Buttons
      },
      {
        path: "/components/grid-system",
        name: "نظام الشبكةو",
        mini: "زو",
        component: GridSystem
      },
      {
        path: "/components/panels",
        name: "لوحات",
        mini: "ع",
        component: Panels
      },
      {
        path: "/components/sweet-alert",
        name: "التنبيه الحلو",
        mini: "ومن",
        component: SweetAlert
      },
      {
        path: "/components/notifications",
        name: "إخطارات",
        mini: "ن",
        component: Notifications
      },
      {
        path: "/components/icons",
        name: "الرموز",
        mini: "و",
        component: Icons
      },
      {
        path: "/components/typography",
        name: "طباعة",
        mini: "ر",
        component: Typography
      }
    ]
  },
  {
    collapse: true,
    path: "/forms",
    name: "إستمارات",
    state: "openForms",
    icon: "content_paste",
    views: [
      {
        path: "/forms/regular-forms",
        name: "أشكال منتظمة",
        mini: "صو",
        component: RegularForms
      },
      {
        path: "/forms/extended-forms",
        name: "أشكال موسعة",
        mini: "هوو",
        component: ExtendedForms
      },
      {
        path: "/forms/validation-forms",
        name: "نماذج التحقق",
        mini: "تو",
        component: ValidationForms
      },
      { path: "/forms/wizard", name: "ساحر", mini: "ث", component: Wizard }
    ]
  },
  {
    collapse: true,
    path: "/tables",
    name: "الجداول",
    state: "openTables",
    icon: GridOn,
    views: [
      {
        path: "/tables/regular-tables",
        name: "الجداول العادية",
        mini: "صر",
        component: RegularTables
      },
      {
        path: "/tables/extended-tables",
        name: "الجداول الموسعة",
        mini: "هور",
        component: ExtendedTables
      },
      {
        path: "/tables/react-tables",
        name: "رد فعل الجداول",
        mini: "غ",
        component: ReactTables
      }
    ]
  },
  {
    collapse: true,
    path: "/maps",
    name: "خرائط",
    state: "openMaps",
    icon: Place,
    views: [
      {
        path: "/maps/google-maps",
        name: "خرائط جوجل",
        mini: "زم",
        component: GoogleMaps
      },
      {
        path: "/maps/full-screen-maps",
        name: "خريطة كاملة الشاشة",
        mini: "ووم",
        component: FullScreenMap
      },
      {
        path: "/maps/vector-maps",
        name: "سهم التوجيه، الخريطة",
        mini: "تم",
        component: VectorMap
      }
    ]
  },
  { path: "/widgets", name: "الحاجيات", icon: WidgetsIcon, component: Widgets },
  {
    path: "/charts",
    name: "الرسوم البيانية",
    icon: Timeline,
    component: Charts
  },
  { path: "/calendar", name: "التقويم", icon: DateRange, component: Calendar }
];

export { sidebarLinks };

export default rtlRoutes;
