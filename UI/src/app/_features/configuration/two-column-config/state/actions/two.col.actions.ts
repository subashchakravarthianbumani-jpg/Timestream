import { createAction, props } from '@ngrx/store';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import {
  IConfigCategoryModel,
  IConfigurationModel,
} from 'src/app/_models/configuration/configuration';
import { TCModel } from 'src/app/_models/user/usermodel';

export const getCategoryList = createAction(
  '[Master Configuration] Get Category List'
);
export const updatesaveStatus = createAction(
  '[Master Configuration] Save Status Updtae List'
);
export const Reset = createAction('[Master Configuration] Reset Everything');
export const getCategoryListSuccess = createAction(
  '[Master Configuration] Get Category List Success',
  props<{ categories: IConfigCategoryModel[] }>()
);

export const getConfigurationDetailsbyId = createAction(
  '[Master Configuration] Get Configuration Details',
  props<{
    departmentId: string;
    configId?: string;
    categoryId?: string;
    parentConfigId?: string;
    isActive?: boolean;
  }>()
);
export const getConfigurationDetailsbyIdSuccess = createAction(
  '[Master Configuration] Get Configuration Details Success',
  props<{ configDetail: IConfigurationModel[] }>()
);

export const getParentConfigurationDetailsbyId = createAction(
  '[Master Configuration] Get Parent Configuration Details',
  props<{
    departmentId: string;
    configId?: string;
    categoryId?: string;
    parentConfigId?: string;
  }>()
);
export const getParentConfigurationDetailsbyIdSuccess = createAction(
  '[Master Configuration] Get Parent Configuration Details Success',
  props<{ configDetail: IConfigurationModel[] }>()
);

export const saveConfiguration = createAction(
  '[Master Configuration] Save Configuration',
  props<{ configDetails: IConfigurationModel }>()
);
export const saveConfigurationSuccess = createAction(
  '[Master Configuration] Save Configuration Success',
  props<{ details: ResponseModel }>()
);

export const getDepartmentList = createAction(
  '[Master Configuration] Get Department List'
);
export const getDepartmentListSuccess = createAction(
  '[Master Configuration] Get Department List Success',
  props<{ departments: TCModel[] }>()
);
