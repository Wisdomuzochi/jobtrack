export enum CandidatureStatut {
  Envoyee = 0,
  EnCours = 1,
  Entretien = 2,
  Refuse = 3,
  Accepte = 4
}

export const STATUT_LABELS: Record<CandidatureStatut, string> = {
  [CandidatureStatut.Envoyee]: 'Envoyée',
  [CandidatureStatut.EnCours]: 'En cours',
  [CandidatureStatut.Entretien]: 'Entretien',
  [CandidatureStatut.Refuse]: 'Refusée',
  [CandidatureStatut.Accepte]: 'Acceptée'
};

// Ordre logique de progression : depuis Envoyee/EnCours/Entretien on ne propose
// que les statuts "en avant". Refuse/Accepte sont des états finaux.
export const STATUTS_SUIVANTS_POSSIBLES: Record<CandidatureStatut, CandidatureStatut[]> = {
  [CandidatureStatut.Envoyee]: [
    CandidatureStatut.Envoyee,
    CandidatureStatut.EnCours,
    CandidatureStatut.Entretien,
    CandidatureStatut.Refuse
  ],
  [CandidatureStatut.EnCours]: [
    CandidatureStatut.EnCours,
    CandidatureStatut.Entretien,
    CandidatureStatut.Refuse
  ],
  [CandidatureStatut.Entretien]: [
    CandidatureStatut.Entretien,
    CandidatureStatut.Refuse,
    CandidatureStatut.Accepte
  ],
  [CandidatureStatut.Refuse]: [CandidatureStatut.Refuse],
  [CandidatureStatut.Accepte]: [CandidatureStatut.Accepte]
};

export interface CompetenceRequise {
  id: string;
  nom: string;
}

export interface Contact {
  id?: string;
  nom: string;
  email: string;
  lienLinkedIn: string;
  role: string;
}

export interface Candidature {
  id: string;
  poste: string;
  entreprise: string;
  lienOffre: string;
  datePublicationOffre: string | null;
  dateCandidature: string;
  statut: CandidatureStatut;
  notes: string;
  competencesRequises: CompetenceRequise[];
  contacts: Contact[];
  relanceConseillee: boolean;
}

export interface CreerCandidatureRequete {
  poste: string;
  entreprise: string;
  lienOffre: string;
  competences: string[];
  contacts: Contact[];
}

export interface ChangerStatutRequete {
  nouveauStatut: CandidatureStatut;
}
