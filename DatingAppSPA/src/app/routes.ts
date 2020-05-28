import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './Resolvers/member-detail.resolver';
import { MemberListResolver } from './Resolvers/member-list.resolver';

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
            {path: 'messages', component: MessagesComponent},
            {path: 'lists', component: ListsComponent}
        ]
    },
   
    {path: '**', redirectTo: '', pathMatch: 'full'}      
];
