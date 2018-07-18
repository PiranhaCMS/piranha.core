import { Component, Input } from '@angular/core';

@Component({
  selector: 'block',
  templateUrl: './block.component.html'
})

export class BlockComponent {

  @Input() model: any;

  constructor() { }
}
