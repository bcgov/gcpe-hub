module MediaRequest {

    // **********************************************************
    // ***   RouterHandler                                   ***
    // **********************************************************

    export interface RouteParams { //parameters that the router gives to the route handler.
        urlParams: Array<string>,
        data: any,  //you can pass in any object when navigating into a route.
        isPush: boolean
    }

    export interface NavParams { //inbound parameters to the router
        path: string,
        data: any, //pass a whole viewmodel through the router
        confirmed?: boolean //do we need to confirm leaving the current state. when true, no confirming required
        isPop?: boolean //does the new route need to be pushed to the history. (not needed when we navigate there from a pop)
    }

    export interface Route {
        id: string;
        routeInit(p: RouteParams): void;
        confirmLeaveFx(): ConfirmResult ;
        windowTitle: string;
        url: string;
        isDefault?: boolean,
        pageUnload?(): void;
        parentRoute?: Route;
    }

    export interface RouteMatch {
        matchedRoute: Route;
        paramValues: Array<string>;
    }

    export class PvmRouter {
        private routes: Array<Route> = [];
        private currentState: Route;

        constructor(private pvm: MediaRequestPvm) {
        }

        public addRoute(r: Route) {
            this.routes.push(r);
        }

        public getRoute(id: string) {
            let r: Route;
            for (var i = 0; i < this.routes.length; i++) {
                if (this.routes[i].id = id) {
                    r = this.routes[i];
                    break;
                }
            }
            return r;
        }

        private matchRoute(matchPath: string): RouteMatch {
            let ret: Route;
            let defaultRoute: Route;
            const p_Arr: Array<string> = matchPath.split('/');
            let paramVals: Array<string> = [];
           
            for (var i = 0; i < this.routes.length; i++) {
                const route: Route = this.routes[i];
                if (route.isDefault) {
                    defaultRoute = route;
                }
                const urlParts = route.url.split('/');
                paramVals = [];
                let partMatch: boolean = false;

                var j = 0;
                while (j < urlParts.length && j < p_Arr.length) {
                    //let's see if this url part is defined as a parameter
                    partMatch = false;
                    if (urlParts[j].indexOf(":") == 0) {
                        paramVals.push(p_Arr[j]);
                        partMatch = true;
                    } else if (urlParts[j] == p_Arr[j]) {
                        //more to come, but we'll go with it for now
                        partMatch = true;
                        
                    }
                    if (!partMatch) {
                        //no match on the route
                        break;
                    }
                    j++;
                }
                if (partMatch && j == urlParts.length && j == p_Arr.length) {
                    //this is the one.
                    ret = route;
                    break;
                }
            }
        //we need to return a copy of the route, so that the original does not get tampered with (remove the confirmLeaveFx forr instance)

            let rt: Route;
            if (!ret) {
                console.log("No route defined for ", matchMedia);
                ret = defaultRoute;
            }
            if (ret) {
                rt = {
                    id: ret.id,
                    routeInit: ret.routeInit,
                    url: matchPath,
                    windowTitle: ret.windowTitle,
                    confirmLeaveFx: ret.confirmLeaveFx,
                    pageUnload: ret.pageUnload
                }
            } else {
                console.log("No default route setup, no match for ", matchPath);
                throw Error("Unable to route to " + matchPath + " and no default defined");

            }
            return { matchedRoute: rt, paramValues: paramVals };
        }

        public navigateTo(np: NavParams) { //path: string, route_data: any, pushState: boolean = true
            //let's see if we can leave
            if (this.currentState && !np.confirmed) {

                //make sure that we can leave the current state. if not, we will confirm with a dialog.
                if (this.pvm.navConfirm().confirmingNavWithModal(np, this.currentState)) {

                    //and get out of here
                    return;
                }
            }
            //first match the path to a known route
            const rm: RouteMatch = this.matchRoute(np.path);

            //then construct the RouteParams
            if (rm.matchedRoute) {
                let p: RouteParams = {urlParams: rm.paramValues , data: np.data, isPush: !np.isPop};
                //then setup the currentState
                if (this.currentState && this.currentState.pageUnload) {
                    //If we're navigating from currentState to a child route, don't unload.
                    const isChildRoute = rm.matchedRoute.url.indexOf(this.currentState.url) === 0;
                    if (isChildRoute) {
                        // Save the parent's current State for the navigation back out of the child!
                        rm.matchedRoute.parentRoute = this.currentState;
                    } else {
                        this.currentState.pageUnload();
                    }
                }

                this.currentState = rm.matchedRoute;
                //call the init
                this.pvm.pageError(undefined); //clear the error of the last page
                rm.matchedRoute.routeInit(p);
                
                let state: any = {
                    id: rm.matchedRoute.id,
                    url: np.path
                }
                if (!np.isPop) { //we turn this off when we call this method from the popstate
                    //and push the history state.
                    history.pushState(state, rm.matchedRoute.windowTitle, state.url);
                }
            }
            window['snowplow']('trackPageView');
        }

        public confirmedNavBack() {
            //remove the confirmFx on the current state, so we can pop it off the stack and actually make it through the onpopstate.
            if (this.currentState) {
                this.currentState.confirmLeaveFx = undefined;
            }
            history.back();
        }

        // Manage what to do on Browser history events.
        public onpopstate = (event) => {
            //console.log("pop");

            var popUrl: string = "";
            if (event && event.state && event.state.url) {
                popUrl = event.state.url;
            }

            var fromState: any;
            let np: NavParams = {
                path: popUrl,
                data: null,
                isPop: true
            };


            let isParentRoute: boolean = false;

            if (this.currentState) {

                fromState = this.currentState;
                //make sure that we can leave the current state. if not, we will confirm with a dialog.
                //console.log("unring pop 1 ", this.modalActive);
                if (this.modalActive || this.pvm.navConfirm().confirmingNavWithModal(np, fromState)) {
                    //unring the bell on the history->back piece
                    // console.log("unring pop 2 ", this.modalActive);
                    history.pushState(fromState.state, fromState.windowTitle, fromState.url);

                    //and get out of here
                    return;
                }

                // If we have a pageUnload method, call it.
                if (this.currentState.pageUnload) {
                    this.currentState.pageUnload();
                }

                // If popstate is parent of the currentState, don't navigateTo.
                isParentRoute = (this.currentState.url.indexOf(np.path) === 0 && this.currentState.url !== np.path);

                this.currentState = this.currentState.parentRoute;
            }


            if (!isParentRoute) {
                this.navigateTo(np);
            }
           
        } //end onpopstate

        private _modalActive: boolean = false;
        get modalActive(): boolean {
            return this._modalActive;
        }
        set modalActive(ma: boolean) {
            //console.log("set modal active to: ", ma);
            this._modalActive = ma;
        }

    }
}