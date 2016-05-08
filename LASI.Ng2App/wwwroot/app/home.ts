import { Component, OnInit, Injectable } from 'angular2/core';
import { Router, RouteConfig, ROUTER_PROVIDERS, CanActivate, RouterOutlet } from 'angular2/router';
import { UserService } from './user-service';
import { LoginComponent } from './login';
import { ListItemComponent } from './document-list/list-item';
import { DocumentViewerComponent } from './document-viewer/components';

@Component({
    selector: 'home',
    template: `<div>
                 <router-outlet></router-outlet>
                 <router-outlet [name]="navbar"></router-outlet>
                 <router-outlet [name]="main"></router-outlet>
               </div>`
})
@RouteConfig([
    { path: 'list', name: 'List', component: ListItemComponent },
    // { path: '/documents', name: 'Documents', component: DocumentViewerComponent }

])
@Injectable()
export class HomeComponent implements OnInit {
    constructor(private userService: UserService, private router: Router) { }

    ngOnInit() {
        if (!this.userService.loggedIn) {
            this.router.navigate(['Login']);
        }
        else {
            this.router.navigate(['Home'])
        }
    }
}