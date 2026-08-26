import { Component, inject, signal } from '@angular/core';
import {
  FormArray,
  FormControl,
  FormGroup,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router } from '@angular/router';

import { Contact, CreerCandidatureRequete } from '../../models/candidature.model';
import { CandidaturesService } from '../../services/candidatures.service';

type ContactGroup = FormGroup<{
  nom: FormControl<string>;
  email: FormControl<string>;
  lienLinkedIn: FormControl<string>;
  role: FormControl<string>;
}>;

@Component({
  selector: 'app-candidature-create',
  imports: [ReactiveFormsModule],
  templateUrl: './candidature-create.html',
  styleUrl: './candidature-create.css'
})
export class CandidatureCreate {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly candidaturesService = inject(CandidaturesService);
  private readonly router = inject(Router);

  protected readonly envoiEnCours = signal(false);
  protected readonly erreur = signal<string | null>(null);

  protected readonly form = this.fb.group({
    poste: this.fb.control('', Validators.required),
    entreprise: this.fb.control('', Validators.required),
    lienOffre: this.fb.control(''),
    competences: this.fb.array<FormControl<string>>([]),
    contacts: this.fb.array<ContactGroup>([])
  });

  protected get competences(): FormArray<FormControl<string>> {
    return this.form.controls.competences;
  }

  protected get contacts(): FormArray<ContactGroup> {
    return this.form.controls.contacts;
  }

  protected ajouterCompetence(): void {
    this.competences.push(this.fb.control('', Validators.required));
  }

  protected retirerCompetence(index: number): void {
    this.competences.removeAt(index);
  }

  protected ajouterContact(): void {
    const groupe: ContactGroup = this.fb.group({
      nom: this.fb.control('', Validators.required),
      email: this.fb.control('', [Validators.required, Validators.email]),
      lienLinkedIn: this.fb.control(''),
      role: this.fb.control('')
    });
    this.contacts.push(groupe);
  }

  protected retirerContact(index: number): void {
    this.contacts.removeAt(index);
  }

  protected onSubmit(event: Event): void {
    event.preventDefault();

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.erreur.set(null);
    this.envoiEnCours.set(true);

    const valeurs = this.form.getRawValue();
    const requete: CreerCandidatureRequete = {
      poste: valeurs.poste,
      entreprise: valeurs.entreprise,
      lienOffre: valeurs.lienOffre,
      competences: valeurs.competences,
      contacts: valeurs.contacts as Contact[]
    };

    this.candidaturesService.creer(requete).subscribe({
      next: () => {
        this.router.navigate(['/candidatures']);
      },
      error: () => {
        this.envoiEnCours.set(false);
        this.erreur.set(
          "La création de la candidature a échoué. Vérifie que l'API JobTrack est démarrée."
        );
      }
    });
  }
}
