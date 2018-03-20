import { Routes, RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { ContactsComponent } from './sections/contacts/contacts.component';
import { AuthGuard } from './auth/auth.guard';

const appRoutes: Routes = [
    { path: 'login', component: LoginComponent },
    // { path: '', component: ContactsComponent },
    { path: '', component: ContactsComponent, canActivate: [AuthGuard] },
    // otherwise redirect to home
    { path: '**', redirectTo: '' }
];

export const routing = RouterModule.forRoot(appRoutes);
