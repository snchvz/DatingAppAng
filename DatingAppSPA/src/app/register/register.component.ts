import { Component, OnInit, Input, Output, EventEmitter, TestabilityRegistry } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker/public_api';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  //example for getting values from parent compoenent
  //@Input() valuesFromHome: any; //this is coming from the parent component (home component)
  
  //use output for passing properties from child to parent component
  @Output() cancelRegister = new EventEmitter(); //output properties emit events
  user: User;
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>; //partial means make all properties in type "optional"

  constructor(private authService: AuthService, 
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router) { }

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-red'
    };
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['',Validators.required],
      knownAs: ['',Validators.required],
      dateOfBirth: [null,Validators.required],
      city: ['',Validators.required],
      country: ['',Validators.required],
      password: ['',[Validators.required, Validators.minLength(4), Validators.maxLength(16)]],
      confirmPassword: ['', Validators.required]
    }, {
      validator: this.passwordMatchValidator
    });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch' : true};
  }

  register(){
    if(this.registerForm.valid)
    {
      this.user = Object.assign({}, this.registerForm.value); //get values from registerForm, assign them to empty object, then assign that object to user
      this.authService.register(this.user).subscribe(() => {
        this.alertify.success("Registration Successful!");
      }, error => {
        this.alertify.error(error);
      }, () => {
        this.authService.login(this.user).subscribe(() => {
          this.router.navigate(['/members']);
        });
      });
    }

    // previous register example
    // this.authService.register(this.model).subscribe(() => {
    //   this.alertify.success('resgistration successful');
    // }, error => {
    //   this.alertify.error(error);
    // });
  }

  cancel()
  {
    this.cancelRegister.emit(false);
    console.log("cancel");
  }

}
