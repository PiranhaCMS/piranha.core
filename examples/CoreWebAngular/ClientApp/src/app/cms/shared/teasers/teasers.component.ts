import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { fadeInAnimation } from '../fade-in.animation';

@Component({
  selector: 'teasers',
  templateUrl: './teasers.component.html',
  animations: [fadeInAnimation],
  host: { '[@fadeInAnimation]': "" }
})

export class TeasersComponent {

  private _model: any;

  @Input()
  set model(val: any) {
    this._model = val;
    this.hasTeasers = val.length > 0;
    this.teaserWidth = this.hasTeasers ? 12 / val.length : 0;
  };

  get model(): any {
    return this._model;
  }

  teaserWidth: number;
  hasTeasers: boolean;

  constructor() { }
}
