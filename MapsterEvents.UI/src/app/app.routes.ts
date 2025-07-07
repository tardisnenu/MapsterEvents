import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  // Public routes
  {
    path: '',
    loadComponent: () => import('./components/home/home.component').then(c => c.HomeComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('./components/auth/login/login.component').then(c => c.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./components/auth/register/register.component').then(c => c.RegisterComponent)
  },
  {
    path: 'events',
    loadComponent: () => import('./components/events/event-list/event-list.component').then(c => c.EventListComponent)
  },
  {
    path: 'events/:id',
    loadComponent: () => import('./components/events/event-detail/event-detail.component').then(c => c.EventDetailComponent)
  },
  {
    path: 'events/category/:categoryId',
    loadComponent: () => import('./components/events/event-list/event-list.component').then(c => c.EventListComponent)
  },
  {
    path: 'events/upcoming',
    loadComponent: () => import('./components/events/event-list/event-list.component').then(c => c.EventListComponent)
  },
  {
    path: 'categories',
    loadComponent: () => import('./components/categories/category-list/category-list.component').then(c => c.CategoryListComponent)
  },
  
  // Protected routes
  {
    path: 'profile',
    loadComponent: () => import('./components/profile/profile.component').then(c => c.ProfileComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'create-event',
    loadComponent: () => import('./components/events/event-create/event-create.component').then(c => c.EventCreateComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'my-events',
    loadComponent: () => import('./components/events/my-events/my-events.component').then(c => c.MyEventsComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'my-registrations',
    loadComponent: () => import('./components/events/my-registrations/my-registrations.component').then(c => c.MyRegistrationsComponent),
    canActivate: [AuthGuard]
  },
  
  // Redirect and 404
  {
    path: '404',
    loadComponent: () => import('./components/shared/not-found/not-found.component').then(c => c.NotFoundComponent)
  },
  {
    path: '',
    redirectTo: '',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: '/404'
  }
];
