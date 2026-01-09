import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { ResponseModel } from '../_models/ResponseStatus';
import { GoFilterModel } from '../_models/filterRequest';
import { httpOptions } from '../_models/utils';
import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Injectable({ providedIn: 'root' })
export class GeneralService {

  // updated by Sivasankar K on 04/12/2025  for slide view 
    cameras: any[] = [];
  selectedIndex: number = 0;
  constructor(private router: Router, private http: HttpClient) {}
  setState(list: any[], index: number) {
    this.cameras = list;
    this.selectedIndex = index;
  }

  getState() {
  return {
    cameras: this.cameras,
    selectedIndex: this.selectedIndex
  };
}
  downloads(id: string, filename: string) {
    var fileUrl = `${environment.apiUrl}/Common/DownloadImage?fileId=${id}`;
    this.DocumentsDownload(fileUrl).subscribe(async (event) => {
      let data = event as HttpResponse<Blob>;
      const downloadedFile = new Blob([data.body as BlobPart], {
        type: data.body?.type,
      });
      if (downloadedFile.type != '') {
        const a = document.createElement('a');
        a.setAttribute('style', 'display:none;');
        document.body.appendChild(a);
        a.download = filename;
        a.href = URL.createObjectURL(downloadedFile);
        a.target = '_blank';
        a.click();
        document.body.removeChild(a);
      }
    });
  }
 
ViewPdf(id: string) {
  const fileUrl = `${environment.apiUrl}/Common/DownloadImage?fileId=${id}`;
  return this.http.get(fileUrl, {
    responseType: 'blob'
  });
}
showPdf(id: string, filename: string) {
  const fileUrl = `${environment.apiUrl}/Common/DownloadImage?fileId=${id}`;
  return this.http.get(fileUrl, {
    responseType: 'blob'
  });
}



  DocumentsDownload(fileUrl: string) {
    return this.http.get(fileUrl, {
      reportProgress: true,
      observe: 'events',
      responseType: 'blob',
    });
  }

  uploadfile() {}
}
//add pip for view pdf

@Pipe({ name: 'safeUrl' })
export class SafeUrlPipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) {}
  transform(url: string): SafeResourceUrl {
    return this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }

  
}
