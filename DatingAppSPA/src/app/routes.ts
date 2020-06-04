import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './Resolvers/member-detail.resolver';
import { MemberListResolver } from './Resolvers/member-list.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './Resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { ListResolver } from './Resolvers/lists.resolver';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent},  
    {
        path: '', //for example, path -> '' + 'members' will return path 'members'
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],   
        children: [ //the routes in children will be protected by AuthGuard
            {path: 'members', component: MemberListComponent, 
                resolve: {users: MemberListResolver}},    
            {path: 'members/:id', component: MemberDetailComponent, 
                resolve: {user: MemberDetailResolver}},    
            {path: 'member/edit', component: MemberEditComponent, 
                resolve: {user: MemberEditResolver}, //instead of doing members/edit/:id, we will get id from decoded token of logged in user
                canDeactivate: [PreventUnsavedChanges]}, 
            {path: 'messages', component: MessagesComponent},
            {path: 'lists', component: ListsComponent, 
                resolve: {users: ListResolver}}
        ]
    },
   
    {path: '**', redirectTo: '', pathMatch: 'full'}      
];
