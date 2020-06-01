import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm', {static: true}) editForm: NgForm;
  @HostListener('window:beforeunload', ['$event'])  //browser specfic popup when you try to back out of page or close page
  unloadNotification($event:any)
  {
    if(this.editForm.dirty)
    {
      $event.returnValue = true;
    }
  }
  user: User;
  photoUrl: string;

  constructor(private route: ActivatedRoute, 
    private alertify: AlertifyService,
    private userService: UserService,
    private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data => { //user route to provide data for user
      this.user = data['user'];
    });
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  updateUser()
  {
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next => {
      this.alertify.success('Profile updated successfully!');
    //reset form so its not "dirty" and save changes button and top alert do not show up when changes are updated
      this.editForm.reset(this.user); //use param (this.user) to keep the values that were changed (and NOT wipe out the values in the form)
    }, error => {
      this.alertify.error(error);
    })
  }

  updateMainPhoto(photoUrl){
    this.user.photoUrl = photoUrl;
  }
}
