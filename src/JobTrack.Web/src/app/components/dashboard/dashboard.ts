import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import {
  Candidature,
  CandidatureStatut,
  STATUTS_SUIVANTS_POSSIBLES,
  STATUT_LABELS
} from '../../models/candidature.model';
import { CandidaturesService } from '../../services/candidatures.service';
import { ConfirmDialog } from '../shared/confirm-dialog/confirm-dialog';

const BADGE_CLASSES: Record<CandidatureStatut, string> = {
  [CandidatureStatut.Envoyee]: 'badge-envoyee',
  [CandidatureStatut.EnCours]: 'badge-encours',
  [CandidatureStatut.Entretien]: 'badge-entretien',
  [CandidatureStatut.Refuse]: 'badge-refuse',
  [CandidatureStatut.Accepte]: 'badge-accepte'
};

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, ConfirmDialog, DatePipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  private readonly candidaturesService = inject(CandidaturesService);

  protected readonly candidatures = signal<Candidature[]>([]);
  protected readonly chargement = signal(true);
  protected readonly erreur = signal<string | null>(null);
  protected readonly candidatureASupprimer = signal<Candidature | null>(null);

  protected readonly statutLabels = STATUT_LABELS;
  protected readonly candidatureStatut = CandidatureStatut;

  protected readonly total = computed(() => this.candidatures().length);
  protected readonly nombreEnCours = computed(
    () => this.candidatures().filter((c) => c.statut === CandidatureStatut.EnCours).length
  );
  protected readonly nombreEntretien = computed(
    () => this.candidatures().filter((c) => c.statut === CandidatureStatut.Entretien).length
  );
  protected readonly nombreRelances = computed(
    () => this.candidatures().filter((c) => c.relanceConseillee).length
  );

  ngOnInit(): void {
    this.charger();
  }

  private charger(): void {
    this.chargement.set(true);
    this.erreur.set(null);

    this.candidaturesService.lister().subscribe({
      next: (candidatures) => {
        this.candidatures.set(candidatures);
        this.chargement.set(false);
      },
      error: () => {
        this.erreur.set(
          "Impossible de charger les candidatures. Vérifie que l'API JobTrack est démarrée."
        );
        this.chargement.set(false);
      }
    });
  }

  protected badgeClasse(statut: CandidatureStatut): string {
    return BADGE_CLASSES[statut];
  }

  protected optionsStatut(candidature: Candidature): CandidatureStatut[] {
    return STATUTS_SUIVANTS_POSSIBLES[candidature.statut];
  }

  protected statutFige(candidature: Candidature): boolean {
    return (
      candidature.statut === CandidatureStatut.Refuse ||
      candidature.statut === CandidatureStatut.Accepte
    );
  }

  protected onChangerStatut(candidature: Candidature, event: Event): void {
    const nouveauStatut = Number(
      (event.target as HTMLSelectElement).value
    ) as CandidatureStatut;

    if (nouveauStatut === candidature.statut) {
      return;
    }

    this.candidaturesService.changerStatut(candidature.id, nouveauStatut).subscribe({
      next: (miseAJour) => {
        // Le endpoint PUT ne recharge pas les collections liées (compétences,
        // contacts) : on ne fusionne que les champs réellement modifiés pour
        // ne pas les faire disparaître de la ligne.
        this.candidatures.update((liste) =>
          liste.map((c) =>
            c.id === miseAJour.id
              ? { ...c, statut: miseAJour.statut, relanceConseillee: miseAJour.relanceConseillee }
              : c
          )
        );
      },
      error: () => {
        this.erreur.set("La mise à jour du statut a échoué.");
      }
    });
  }

  protected demanderSuppression(candidature: Candidature): void {
    this.candidatureASupprimer.set(candidature);
  }

  protected annulerSuppression(): void {
    this.candidatureASupprimer.set(null);
  }

  protected confirmerSuppression(): void {
    const candidature = this.candidatureASupprimer();
    if (!candidature) {
      return;
    }

    this.candidaturesService.supprimer(candidature.id).subscribe({
      next: () => {
        this.candidatures.update((liste) => liste.filter((c) => c.id !== candidature.id));
        this.candidatureASupprimer.set(null);
      },
      error: () => {
        this.erreur.set('La suppression a échoué.');
        this.candidatureASupprimer.set(null);
      }
    });
  }
}
