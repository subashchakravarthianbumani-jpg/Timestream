import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RoundPipe } from '../pipes/InitCaps';

@NgModule({
  declarations: [RoundPipe],
  imports: [CommonModule],
  exports: [RoundPipe],
})
export class PipeModuleModule {}
