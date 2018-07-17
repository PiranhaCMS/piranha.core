import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from "@angular/core";
import { Meta, Title } from "@angular/platform-browser";
import { NavigationStart, Router } from "@angular/router";
import { Observable, Subject, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class CmsService {

  private routeCache: any[] = [];

  public static url: string

  sitemapChanged: Subject<any> = new Subject<any>();
  modelChanged: Subject<any[]> = new Subject<any[]>();
  loadingChanged: Subject<boolean> = new Subject<boolean>();
  private errors: any;
  private sitemap: any;
  private subSitemap: any;
  private model: any;
  private currentPage: string;

  constructor(private http: HttpClient, private router: Router, private meta: Meta, private title: Title) {

    this.currentPage = router.url;   

    router.events.subscribe((val) => {
      if (val instanceof NavigationStart) {
        this.currentPage = val.url;
        this.getModel();
      }
    });

    this.int();
  }

  public int() {
    this.getSiteMap()
      .subscribe((result) => this.onSuccessfulGetSiteMap(result),
        (errors: any) => this.onUnsuccessful(errors)
      );
  }

  private getModel() {
    if (!this.sitemap || !this.currentPage)
      return;

    //this.loadingChanged.next(true);

    let route = this.getRouteId(this.sitemap, this.currentPage);
    this.subSitemap = route.Items;
    let model = this.routeCache.find(model => {
      return model.Id === route.Id;
    });
    if (model) {
      //timeout is to allow the view to load before sending the model from cache
      setTimeout(() => {
        this.onSuccessfulGetModel(model, true);
        this.loadingChanged.next(false);
      }, 50);
    } else if (route.PageTypeName === "Teaser Page") {
      this.getTeaserPage(route.Id)
        .subscribe((result) => this.onSuccessfulGetModel(result),
          (errors: any) => this.onUnsuccessful(errors),
          () => this.loadingChanged.next(false));
    } else if (route.PageTypeName === "Blog Archive") {
      this.getArchive(route.Id)
        .subscribe((result) => this.onSuccessfulGetModel(result),
          (errors: any) => this.onUnsuccessful(errors),
          () => this.loadingChanged.next(false));
    } else if (route.PageTypeName === "BlogPost") {
      this.getPost(route.Id)
        .subscribe((result) => this.onSuccessfulGetModel(result),
          (errors: any) => this.onUnsuccessful(errors),
          () => this.loadingChanged.next(false));
    } else if (route.PageTypeName === "Standard page") {
      this.getPage(route.Id)
        .subscribe((result) => this.onSuccessfulGetModel(result),
          (errors: any) => this.onUnsuccessful(errors),
          () => this.loadingChanged.next(false));
    }
  }

  private getRouteId(routes: any, route: string): any {
    for (let route of routes) {
      if (route.Permalink === this.currentPage)
        return route;
      if (route.Items.length >= 0) {
        let id = this.getRouteId(route.Items, route);
        if (id != null)
          return id;
      }
    }
    return null;
  }

  private onSuccessfulGetSiteMap(result): void {
    this.sitemap = result;
    console.log(this.sitemap);
    this.sitemapChanged.next(this.sitemap);
  }

  private onSuccessfulGetModel(result: any, fronCache: boolean = false) {
    if (!fronCache) {
      this.routeCache.push(result);
    }
    if (result.RedirectUrl && result.RedirectUrl !== "") {
      document.location.replace(result.RedirectUrl);
    } else {
      this.model = result;
      this.modelChanged.next([this.model, this.currentPage, this.subSitemap]);

      this.title.setTitle(this.model.Title);

      this.meta.updateTag({ name: "keywords", content: this.model.MetaKeywords.length > 0 ? this.model.MetaKeywords : "" });

      this.meta.updateTag({ name: "og:title", content: this.model.Title });

      this.meta.updateTag({ name: "description", content: this.model.MetaDescription.length > 0 ? this.model.MetaDescription : "" });

      this.meta.updateTag({ name: "og:description", content: this.model.MetaDescription.length > 0 ? this.model.MetaDescription : "" });
    }
  }

  private onUnsuccessful(result: any) {
    //this.errors = errors;
  }

  private getSiteMap(id: string = null): Observable<any> {
    const url: string = `${CmsService.url}/sitemap?id=${id}`;
    return this.http.get(url)
      .pipe(catchError(this.handleError));
  }

  private getArchive(id: string, year: number = null, month: number = null, page: number = null, category: string = null, tag: string = null): Observable<any> {
    const url: string = `${CmsService.url}/archive?id=${id}&year=${year}&month=${month}&page=${page}&category=${category}&tag=${tag}`;
    return this.http.get(url)
      .pipe(catchError(this.handleError));
  }

  private getPage(id: string): Observable<any> {
    const url: string = `${CmsService.url}/page?id=${id}`;
    return this.http.get(url)
      .pipe(catchError(this.handleError));
  }

  private getPost(id: string): Observable<any> {
    const url: string = `${CmsService.url}/post?id=${id}`;
    return this.http.get(url)
      .pipe(catchError(this.handleError));
  }

  private getTeaserPage(id: string): Observable<any> {
    const url: string = `${CmsService.url}/teaserpage?id=${id}`;
    return this.http.get(url)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
    }
    // return an observable with a user-facing error message
    return throwError(
      'Something bad happened; please try again later.');
  }
}
