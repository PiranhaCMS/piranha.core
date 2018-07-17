import { Component, OnInit, OnDestroy } from '@angular/core';
import { CmsService } from '../cms.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { fadeInAnimation } from '../shared/fade-in.animation';

@Component({
    selector: 'post',
  templateUrl: './post.component.html',
  animations: [fadeInAnimation],
  host: { '[@fadeInAnimation]': "" }
})

export class PostComponent {

  private ngUnsubscribe: Subject<void> = new Subject<void>();
  model: any;
  isLoading: boolean = true;
  constructor(private cmsService: CmsService) {

  }

  ngOnInit(): void {

    this.cmsService.loadingChanged
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((value) => {
        this.isLoading = value;
      });

    this.cmsService.modelChanged
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((value) => {
        this.model = value[0];
      });
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
