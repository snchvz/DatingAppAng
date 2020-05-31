import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from 'src/app/_models/photo';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { JsonPipe } from '@angular/common';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getMemberPhotoChange = new EventEmitter<string>(); //outputs are event emitters
  uploader:FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentMainPhoto: Photo;

  constructor(private authService: AuthService, 
    private userService: UserService,
    private alertify: AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader( {
      url: this.baseUrl + "users/" + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,  //remove from upload queue when after its successfully uploaded
      autoUpload: false, //click button in order to upload
      maxFileSize: 10 * 1024 * 1024 //10MB      
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false; //pass CORS error
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id : res.id,
          url : res.url,
          dateAdded : res.dateAdded,
          description : res.description,
          isMain : res.isMain
        };
        this.photos.push(photo);
      }
    };
  }

  setMainPhoto(photo: Photo){
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id)
      .subscribe(() => {
        this.currentMainPhoto = this.photos.filter(p => p.isMain === true)[0]; //filter out all photos that are not main (should only be 1 photo that is main)
        this.currentMainPhoto.isMain = false;
        photo.isMain = true;
        this.getMemberPhotoChange.emit(photo.url);
      }, error => {
        this.alertify.error(error);
      })
  }
}
