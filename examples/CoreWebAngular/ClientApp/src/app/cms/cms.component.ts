import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs/Subject';
import { takeUntil } from 'rxjs/operators';
import { ArchiveComponent } from './archive/archive.component';
import { CmsService } from './cms.service';
import { ErrorComponent } from './error/error.component';
import { PageComponent } from './page/page.component';
import { PostComponent } from './post/post.component';
import { StartComponent } from './start/start.component';
import { TeaserPageComponent } from './teaser/teaser-page.component';

@Component({
  selector: 'cms',
  templateUrl: './cms.component.html'
})

export class CmsComponent implements OnDestroy {
  isExpanded = false;
  private ngUnsubscribe: Subject<void> = new Subject<void>();
  sitemap: any;
  model: any;
  isLoading: boolean = true;
  currentPage: string;
  currentPageParent: string;
  constructor(private cmsService: CmsService, private router: Router) {
this.cmsService.loadingChanged
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((value) => {
        this.isLoading = value;
      });

    this.cmsService.sitemapChanged
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((value) => {

        let routes = this.router.config
        let parent = routes.find(route => {
          return route.path === ""
        });

        let siteRoutes = [];
        for (let route of value) {
          let link = route.Permalink.substring(1);

          if (route.PageTypeName === "Teaser Page") {
            if (link === "") {
              siteRoutes.push({ path: link, component: StartComponent });
            } else {
              siteRoutes.push({ path: link, component: TeaserPageComponent });
            }
          } else if (route.PageTypeName === "Blog Archive") {
            siteRoutes.push({ path: link, component: ArchiveComponent });
            for (let post of route.Items) {
              siteRoutes.push({ path: post.Permalink.substring(1), component: PostComponent });
            }
          } if (route.PageTypeName === "Standard page") {
            siteRoutes.push({ path: link, component: PageComponent });
          }         
        }
        siteRoutes.push({ path: "**", component: ErrorComponent });

        parent.children = siteRoutes;

        this.router.resetConfig(routes);

        this.sitemap = value;
      });

    this.cmsService.modelChanged
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((value) => {
        this.model = value[0];
        this.currentPage = value[1];
        let parent = `/${this.currentPage.split("/")[1]}`;
        this.currentPageParent = this.currentPage != parent ? parent : "";
      });
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
