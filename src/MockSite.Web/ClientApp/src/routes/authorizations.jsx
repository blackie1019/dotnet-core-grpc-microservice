import UserLoginPage from 'views/Demo/UserLoginPage.jsx';

// @material-ui/icons
import Fingerprint from '@material-ui/icons/Fingerprint';

const authorizationRoutes =  [
    {
        path: '/authorization/login',
        name: 'Logout',
        mini: 'I',
        icon: Fingerprint,
        component: UserLoginPage
    }
];

export default authorizationRoutes;
