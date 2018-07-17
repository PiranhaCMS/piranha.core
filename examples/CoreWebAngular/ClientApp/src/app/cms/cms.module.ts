import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { ArchiveComponent } from './archive/archive.component';
import { CmsComponent } from './cms.component';
import { CmsService } from './cms.service';
import { ErrorComponent } from './error/error.component';
import { PageComponent } from './page/page.component';
import { PostComponent } from './post/post.component';
import { BlockComponent } from './shared/block/block.component';
import { TeasersComponent } from './shared/teasers/teasers.component';
import { FirstParagraphPipe } from './shared/utils';
import { StartComponent } from './start/start.component';
import { TeaserPageComponent } from './teaser/teaser-page.component';
import { WildCardComponent } from './wildcard/wildcard.component';

@NgModule({
  imports: [
    CommonModule,
    BrowserAnimationsModule,
    RouterModule.forRoot([
      {
        path: '', component: CmsComponent,
        children: [
          { path: '**', component: WildCardComponent }
        ]
      }
    ])
  ],
  entryComponents: [
    WildCardComponent,
    CmsComponent,
    ArchiveComponent,
    PageComponent,
    PostComponent,
    StartComponent,
    TeasersComponent,
    TeaserPageComponent,
    ErrorComponent
  ],
  declarations: [
    FirstParagraphPipe,
    WildCardComponent,
    CmsComponent,
    BlockComponent,
    ArchiveComponent,
    PageComponent,
    PostComponent,
    StartComponent,
    TeasersComponent,
    TeaserPageComponent,
    ErrorComponent
  ],
  exports: [
    RouterModule,
    FirstParagraphPipe,
    BlockComponent,
    WildCardComponent,
    CmsComponent,
    ArchiveComponent,
    PageComponent,
    PostComponent,
    StartComponent,
    TeasersComponent,
    TeaserPageComponent,
    ErrorComponent
  ],

})
export class CmsModule {
  static forRoot(apiUrl: string = "api/cms"): ModuleWithProviders {
    CmsService.url = apiUrl;
    return {
      ngModule: CmsModule,
      providers: [CmsService]
    };
  }
}
