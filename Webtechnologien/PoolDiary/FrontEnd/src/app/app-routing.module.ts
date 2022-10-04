import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AddEntryComponent } from './components/add-entry/add-entry.component';
import { LoginComponent } from './components/login/login.component';
import { PoolDiaryComponent } from './components/pool-diary/pool-diary.component';
import { RegisterComponent } from './components/register/register.component';
import { GuardHomeRouteService as HomeRouteGuardService } from './services/guard-home-route.service';

// Hier alle Routes konfigurieren, inklusive CanActivate für Views die nur Authorisierte benutzer sehen sollen.
const routes: Routes = [
  {path:'home', component:PoolDiaryComponent, canActivate : [HomeRouteGuardService]},
 {path:'', component:LoginComponent},
 {path:'login', component:LoginComponent},
 {path:'register', component:RegisterComponent},
 {path:'addEntry', component:AddEntryComponent, canActivate : [HomeRouteGuardService]}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
