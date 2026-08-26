import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import {
  Candidature,
  CandidatureStatut,
  ChangerStatutRequete,
  CreerCandidatureRequete
} from '../models/candidature.model';

@Injectable({ providedIn: 'root' })
export class CandidaturesService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5027/api/candidatures';

  lister(): Observable<Candidature[]> {
    return this.http.get<Candidature[]>(this.apiUrl);
  }

  creer(requete: CreerCandidatureRequete): Observable<Candidature> {
    return this.http.post<Candidature>(this.apiUrl, requete);
  }

  changerStatut(id: string, nouveauStatut: CandidatureStatut): Observable<Candidature> {
    const requete: ChangerStatutRequete = { nouveauStatut };
    return this.http.put<Candidature>(`${this.apiUrl}/${id}`, requete);
  }

  supprimer(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
