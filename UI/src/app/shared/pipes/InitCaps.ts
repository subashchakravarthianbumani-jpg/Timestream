import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'InitCaps',
})
export class InitCapsPipe implements PipeTransform {
  transform(value: string): string {
    const splitValue = value?.split(' ');
    var finalValue: string = '';
    if (splitValue && splitValue.length > 0) {
      splitValue.forEach((element) => {
        finalValue.concat('');
      });
    }
    return value;
  }
}
@Pipe({ name: 'round' })
export class RoundPipe implements PipeTransform {
  transform(input: number) {
    return Math.floor(input);
  }
}
