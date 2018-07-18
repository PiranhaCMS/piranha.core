import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Pipe({name: 'firstParagraph'})
export class FirstParagraphPipe implements PipeTransform {
  transform(value: string): string {
    
    if (!value || value.length === 0)
      return "";
    let matches = value.match('<p[^>]*>.*?</p>');
    return matches ? matches[0] : "";
  }
}

@Pipe({ name: 'safeHtml' })
export class SafeHtmlPipe {
  constructor(private sanitizer: DomSanitizer) { }

  transform(value) {
    return this.sanitizer.bypassSecurityTrustHtml(value);
  }
}
