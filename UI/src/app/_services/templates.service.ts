import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { TemplateViewModel } from '../_models/configuration/templates';
import { httpOptions } from '../_models/utils';
import { ResponseModel } from '../_models/ResponseStatus';

@Injectable({ providedIn: 'root' })
export class Templateservice {
  constructor(private router: Router, private http: HttpClient) {}


  getTemplates(isActive: boolean, subcategory?: string,mainCategory?: string,serviceType?:string,categoryType?:string) {
   
  let url = `${environment.apiUrl}/Settings/Template_Get?IsActive=${isActive}`;
  if (subcategory) {
    url += `&subcategory=${encodeURIComponent(subcategory)}`;
  }if (mainCategory) {
    url += `&mainCategory=${encodeURIComponent(mainCategory)}`;
  }if (serviceType) {
    url += `&serviceType=${encodeURIComponent(serviceType)}`;
  }if (categoryType) {
    url += `&categoryType=${encodeURIComponent(categoryType)}`;
  }
  return this.http.get<ResponseModel>(url);
}

  getWorkTypes() {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Template_GetWorkTypeList`
    );
  }
   getServiceTypes() {
    
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Template_ServiceTypeList`
    );
  }
   getCategoryTypes() {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Template_CategoryTypeList`
    );
  }
  getConfigurationDetailsbyId(configId?: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Configuration_Get?ParentConfigurationId=${configId}`
    );
  }
  saveTemplates(role: TemplateViewModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Settings/Template_SaveUpdate
      `,
      role,
      httpOptions
    );
  }
  EditWorkTemplates(role: TemplateViewModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Settings/Work_Template_Edit
      `,
      role,
      httpOptions
    );
  }

  get_Template_With_Milestone(
    Id: string,
    IsActive_template?: boolean,
    IsActive_milestone?: boolean,
    WorkTypeId?: string
  ) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Template_Get_With_Milestone?IsActive_template=${IsActive_template}&IsActive_milestone=${IsActive_milestone}&Id=${Id}&WorkTypeId=${WorkTypeId}`
    );
  }
  save_Milestone(milestones: TemplateViewModel[]) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Settings/TemplateMilestone_SaveUpdate
      `,
      milestones,
      httpOptions
    );
  }
  publishTemplate(Id: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Template_Publish?Id=${Id}`
    );
  }



 GetAllDivision(payload: any = {}) {
    return this.http.post<any>(
      `${environment.apiUrl}/Dashboard/GetAllDivision`,
      payload
    );
  }


  // ✅ District
// GetAllDistrict(divisionId: string) {
//   return this.http.post<any>(
//     `${environment.apiUrl}/Dashboard/GetAllDistrict?divisionId=${encodeURIComponent(divisionId)}`,
//     {}
//   );
// }
GetAllDistrict(divisionId: string) {
  return this.http.post<any>(
    `${environment.apiUrl}/Dashboard/GetAllDistrict`,
    { divisionIds: [divisionId] }
  );
}




// ✅ Work Category
GetAllWorkType(divisionId: string, districtId: string) {
  return this.http.post<any>(
    `${environment.apiUrl}/Dashboard/GetAllMainCategory?divisionId=${encodeURIComponent(divisionId)}&districtId=${encodeURIComponent(districtId)}`,
  {});
}


// ✅ Sub Category
GetAllSubWorkType(divisionId: string, districtId: string, mainCategory: string) {
  return this.http.post<any>(
    `${environment.apiUrl}/Dashboard/GetAllSubCategory?divisionId=${encodeURIComponent(divisionId)}&districtId=${encodeURIComponent(districtId)}&mainCategory=${encodeURIComponent(mainCategory)}`,
     {});
  }


// ✅ Work Status
GetAllWorkStatus(
  divisionId: string,
  districtId: string,
  mainCategory: string,
  subCategory: string
) {
  return this.http.post<any>(
    `${environment.apiUrl}/Dashboard/GetAllWorkStatus?divisionId=${encodeURIComponent(divisionId)}&districtId=${encodeURIComponent(districtId)}&mainCategory=${encodeURIComponent(mainCategory)}&subCategory=${encodeURIComponent(subCategory)}`,
{});
}


// ✅ Tender Numbers (Work IDs)
GetAllTenderNumber(
  divisionId: string,
  districtId: string,
  mainCategory: string,
  subCategory: string,
  workStatus: string
) {
  return this.http.post<any>(
    `${environment.apiUrl}/Dashboard/GetAllTenderNumber?divisionId=${encodeURIComponent(divisionId)}&districtId=${encodeURIComponent(districtId)}&mainCategory=${encodeURIComponent(mainCategory)}&subCategory=${encodeURIComponent(subCategory)}&workStatus=${encodeURIComponent(workStatus)}`,
  {});
  }


// GetAllDistrict(payload: any = {}) {
//     return this.http.post<any>(
//       `${environment.apiUrl}/Dashboard/GetAllDistrict`,
//       payload
//     );
//   }


// GetAllWorkType(payload: any = {}) {
//     return this.http.post<any>(
//       `${environment.apiUrl}/Dashboard/GetAllMainCategory`,
//       payload
//     );
//   }


//   GetAllSubWorkType(payload: any = {}) {
//     return this.http.post<any>(
//       `${environment.apiUrl}/Dashboard/GetAllSubCategory`,
//       payload
//     );
//   }


//   GetAllWorkStatus(payload: any = {}) {
//     return this.http.post<any>(
//       `${environment.apiUrl}/Dashboard/GetAllWorkStatus`,
//       payload
//     );
//   }

//   GetAllTenderNumber(payload: any = {}) {
//     return this.http.post<any>(
//       `${environment.apiUrl}/Dashboard/GetAllTenderNumber`,
//       payload
//     );
//   }

}
