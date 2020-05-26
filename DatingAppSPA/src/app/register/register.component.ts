import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

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
  model: any = {};

  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  register(){
    this.authService.register(this.model).subscribe(() => {
      this.alertify.success('resgistration successful');
    }, error => {
      this.alertify.error(error);
    });
  }

  cancel()
  {
    this.cancelRegister.emit(false);
    console.log("cancel");
  }

}
