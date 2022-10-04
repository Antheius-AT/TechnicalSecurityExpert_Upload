import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
 
// Prime Ng related modules.
import { AccordionModule } from 'primeng/accordion';
import { ButtonModule } from 'primeng/button';
import {  InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { HomeComponent } from './components/home/home.component';




@NgModule({
  declarations: [AppComponent, LoginComponent, RegisterComponent, HomeComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    // Prime Ng related modules.
    AccordionModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    PasswordModule,
    ReactiveFormsModule
    

  ],
  providers: [ ],
  bootstrap: [AppComponent],
})
export class AppModule {}
