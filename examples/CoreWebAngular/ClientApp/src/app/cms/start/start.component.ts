import { Component, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CmsService } from '../cms.service';
import { fadeInAnimation } from '../shared/fade-in.animation';

@Component({
    selector: 'start',
  templateUrl: './start.component.html',
  animations: [fadeInAnimation],
  host: { '[@fadeInAnimation]': "" }
})

export class StartComponent implements OnDestroy{

  private ngUnsubscribe: Subject<void> = new Subject<void>();
  model: any;
  isLoading: boolean = true;
  constructor(private cmsService: CmsService) {
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
