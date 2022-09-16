import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from 'src/app/shared/components/button/button.component';
import { InputComponent } from './components/input/input.component';




@NgModule({
  declarations: [ButtonComponent, InputComponent],
  exports: [ButtonComponent, InputComponent]
})
export class SharedModule { }
