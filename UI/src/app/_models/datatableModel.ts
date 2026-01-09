export interface Column {
  field: string;
  header: string;
  customExportHeader?: string;
  isSortable?: boolean;
  sortablefield?: string;
  isSearchable?: boolean;
  isLink?: boolean;
  isBadge?: boolean;
  badgeColor?: string;
  isProgress?: boolean;
  isAnchortagforFilter?: boolean;
  isPopup?: boolean;
  popupType?: string;
  isDownloadable?: boolean;

   isIconButton?: boolean;   
  buttonIcon?: string;     
  actionType?: string; 
}

export interface PageEvent {
  first: number | 0;
  rows: number;
  page: number;
  pageCount: number;
}

export interface ExportColumn {
  title: string;
  dataKey: string;
}

export interface Actions {
  ids?: string[];
  icon: string;
  title: string;
  type: string;
  visibilityCheckFeild?: string;
  privilege?: string | string[];
  isIcon?: boolean;
}

export interface ActionModel {
  type: string;
  record: any;
}
