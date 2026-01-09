import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ThemeService {

  private STORAGE_KEY = 'APP_THEME';
  private lightTheme = 'assets/layout/styles/theme/lara-light-indigo/theme.css';
  private darkTheme  = 'assets/layout/styles/theme/lara-dark-indigo/theme.css';

  constructor() {
    const theme = localStorage.getItem(this.STORAGE_KEY);
    theme === 'LIGHT' ? this.enableLight() : this.enableDark();
  }

  private setTheme(themeHref: string, bodyClass: string) {

    const themeLink = document.getElementById(
      'theme-css'
    ) as HTMLLinkElement;

    if (themeLink) themeLink.href = themeHref;

    document.body.classList.remove('theme-light', 'theme-dark');
    document.body.classList.add(bodyClass);
  }

  /* ✅ LIGHT MODE */
  enableLight() {
    this.setTheme(this.lightTheme, 'theme-light');
    localStorage.setItem(this.STORAGE_KEY, 'LIGHT');
  }

  /* ✅ DARK MODE */
  enableDark() {
    this.setTheme(this.darkTheme, 'theme-dark');
    localStorage.setItem(this.STORAGE_KEY, 'DARK');
  }

  toggle() {
    this.isDark() ? this.enableLight() : this.enableDark();
  }

  isDark(): boolean {
    return localStorage.getItem(this.STORAGE_KEY) !== 'LIGHT';
  }
}
