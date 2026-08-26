import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-confirm-dialog',
  imports: [],
  templateUrl: './confirm-dialog.html',
  styleUrl: './confirm-dialog.css'
})
export class ConfirmDialog {
  @Input() titre = 'Confirmer';
  @Input() message = 'Es-tu sûr de vouloir continuer ?';
  @Input() texteConfirmer = 'Confirmer';
  @Input() texteAnnuler = 'Annuler';

  @Output() confirme = new EventEmitter<void>();
  @Output() annule = new EventEmitter<void>();

  onConfirmer(): void {
    this.confirme.emit();
  }

  onAnnuler(): void {
    this.annule.emit();
  }
}
