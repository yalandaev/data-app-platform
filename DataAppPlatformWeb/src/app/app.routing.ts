import { Routes, RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { ContactsComponent } from './sections/contacts/contacts.component';
import { AuthGuard } from './auth/auth.guard';
import { ContactPageComponent } from './sections/contacts/contact-page.component';

const appRoutes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'contacts', component: ContactsComponent, canActivate: [AuthGuard] },
    { path: 'contacts/:id', component: ContactPageComponent, canActivate: [AuthGuard] },
    // otherwise redirect to home
    { path: '**', redirectTo: 'contacts' }
];

export const routing = RouterModule.forRoot(appRoutes);
