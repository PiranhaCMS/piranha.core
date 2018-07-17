import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CmsService } from '../cms.service';

@Component({
  selector: 'wildcard',
  templateUrl: './wildcard.component.html'
})
 // WildCardComponent to deal with sitemap not being loaded for deep liinking
export class WildCardComponent implements OnDestroy{

  private ngUnsubscribe: Subject<void> = new Subject<void>();
  sitemap: any;
  model: any;
  isLoading: boolean = true;
  currentPage: string;
  currentPageChild: boolean;

  constructor(private cmsService: CmsService, private router: Router) {
this.cmsService.sitemapChanged
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((value) => {
          this.router.navigate([this.router.url])
        }); 
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
