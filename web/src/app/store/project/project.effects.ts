import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, mergeMap, catchError, switchMap } from 'rxjs/operators';
import { ProjectService } from '../../core/services';
import * as ProjectActions from './project.actions';

@Injectable()
export class ProjectEffects {
  private actions$ = inject(Actions);
  private projectService = inject(ProjectService);

  loadProjects$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProjectActions.loadProjects),
      switchMap(() =>
        this.projectService.getAll().pipe(
          map((projects) => ProjectActions.loadProjectsSuccess({ projects })),
          catchError((error) => of(ProjectActions.loadProjectsFailure({ error: error.message })))
        )
      )
    )
  );

  createProject$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProjectActions.createProject),
      mergeMap(({ project }) =>
        this.projectService.create(project).pipe(
          map((newProject) => ProjectActions.createProjectSuccess({ project: newProject })),
          catchError((error) => of(ProjectActions.createProjectFailure({ error: error.message })))
        )
      )
    )
  );

  updateProject$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProjectActions.updateProject),
      mergeMap(({ id, project }) =>
        this.projectService.update(id, project).pipe(
          map(() => ProjectActions.updateProjectSuccess({ project: { ...project, id } as any })),
          catchError((error) => of(ProjectActions.updateProjectFailure({ error: error.message })))
        )
      )
    )
  );

  activateProject$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProjectActions.activateProject),
      mergeMap(({ id }) =>
        this.projectService.activate(id).pipe(
          map(() => ProjectActions.toggleProjectStatusSuccess({ id, isActive: true })),
          catchError((error) =>
            of(ProjectActions.toggleProjectStatusFailure({ error: error.message }))
          )
        )
      )
    )
  );

  deactivateProject$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProjectActions.deactivateProject),
      mergeMap(({ id }) =>
        this.projectService.deactivate(id).pipe(
          map(() => ProjectActions.toggleProjectStatusSuccess({ id, isActive: false })),
          catchError((error) =>
            of(ProjectActions.toggleProjectStatusFailure({ error: error.message }))
          )
        )
      )
    )
  );

  deleteProject$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProjectActions.deleteProject),
      mergeMap(({ id }) =>
        this.projectService.delete(id).pipe(
          map(() => ProjectActions.deleteProjectSuccess({ id })),
          catchError((error) => of(ProjectActions.deleteProjectFailure({ error: error.message })))
        )
      )
    )
  );
}
