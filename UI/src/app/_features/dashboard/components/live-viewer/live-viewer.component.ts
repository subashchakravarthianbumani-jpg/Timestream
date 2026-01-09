import { Component, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GeneralService } from 'src/app/_services/general.service';
import { environment } from 'src/environments/environment';
import { Location } from '@angular/common';
 
 
@Component({
  selector: 'app-live-viewer',
  templateUrl: './live-viewer.component.html',
  styleUrls: ['./live-viewer.component.scss'],
})
export class LiveViewerComponent implements AfterViewInit {
  header = {
    division: '',
    district: '',
    workId: '',
    status: '',
    updated: '',
  };
  // updated by Sivasankar K on 04/12/2025  for slide view
  cameras: any[] = [];
  currentIndex: number = 0;
  isSingleCamera = false;
 
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private state: GeneralService,
    private location: Location
  ) {}
 
  // updated by Sivasankar K on 04/12/2025  for slide view
  ngOnInit() {
    // Full list from service
    this.cameras = this.state.cameras || [];
    this.currentIndex = this.state.selectedIndex || 0;
 
     // ✅ Detect report-grid navigation
  this.isSingleCamera = this.cameras.length === 1;
   
 
    // this.loadStream(this.currentIndex);
  }
 
  loadStream(i: number) {
    const cam = this.cameras[i];
    if (!cam) return;
 
    // const rtspUrl = cam.rtspUrl ? cam.rtspUrl : cam.rtmpUrl;
    // const proxy =
    //   environment.apiUrl.replace('/api', '') +
    //   '/api/settings/live?rtspUrl=' +
    //   encodeURIComponent(rtspUrl);
 
    const streamUrl =
  cam.rtmpUrl && cam.rtmpUrl.trim() !== ''
    ? cam.rtmpUrl
    : cam.rtspUrl;
 
// 🔹 Safety check
if (!streamUrl) {
  console.error('No RTMP or RTSP URL available for camera', cam);
  return;
}
const proxy =
  environment.apiUrl.replace('/api', '') +
  '/api/settings/live?rtspUrl=' +
  encodeURIComponent(streamUrl);
 
    const img = document.getElementById('cam01') as HTMLImageElement;
    img.src = proxy;
 
    // update header
    this.header = {
      division: cam.divisionName,
      district: cam.districtName,
      workId: cam.tenderNumber,
      status: cam.workStatus,
      updated: cam.lastUpdated,
    };
  }
 
 
  // updated by Sivasankar K on 04/12/2025  for slide view
  ngAfterViewInit() {
    const stateData = this.state.getState?.();
    if (
      !stateData ||
      !Array.isArray(stateData.cameras) ||
      stateData.cameras.length === 0
    ) {
      console.error('LiveViewer: no camera data in service state');
      return;
    }
 
    const cams = stateData.cameras;
    // let currentIndex = Number(stateData.selectedIndex) || 0;
    let currentIndex =
  typeof stateData.selectedIndex === 'number'
    ? stateData.selectedIndex
    : cams.findIndex(c => c.rtmpUrl && c.rtmpUrl.trim() !== '');
 
if (currentIndex === -1) currentIndex = 0;
 
    if (currentIndex < 0 || currentIndex >= cams.length) currentIndex = 0;
 
    // Build CSV lists safe for embedding
    // const rtspList = cams.map((c: any) => c.rtspUrl || '').join(',');
    const streamList = cams
  .map((c: any) =>
    c.rtmpUrl && c.rtmpUrl.trim() !== ''
      ? c.rtmpUrl
      : c.rtspUrl || ''
  )
  .join(',');
 
    const divList = cams.map((c: any) => c.divisionName || '').join(',');
    const distList = cams.map((c: any) => c.districtName || '').join(',');
    const workList = cams.map((c: any) => c.tenderNumber || '').join(',');
    const statList = cams.map((c: any) => c.workStatus || '').join(',');
 
    const safeBaseUrl = environment.apiUrl.replace(/\/api\/?$/, '').trim();
 
    const API_BASE_URL = environment.API_BASE_URL;
 
   
 
 
    setTimeout(() => {
      const script = document.createElement('script');
 
      script.innerHTML = `
(function(){
  let currentIndex = ${currentIndex};
  let currentStream = 'main';
  let isPlayback = false;
  let playbackActive = false;
  let currentPlaybackId = null;
 
 
  const streamList = "${streamList}".split(',');
  const divList = "${divList}".split(',');
  const distList = "${distList}".split(',');
  const workList = "${workList}".split(',');
  const statList = "${statList}".split(',');
 
  const camCount = streamList.length;
  const grid = document.getElementById('cameraGrid');
  const layoutSelect = document.getElementById('layoutSelect');
  const slidePrev = document.getElementById('slidePrev');
  const slideNext = document.getElementById('slideNext');
  const closeBtn = document.querySelector('.close-btn');
  const boxes = Array.from(document.querySelectorAll('.cam-box'));
  const resBtns = Array.from(document.querySelectorAll('.res-btn'));
 
  function updateHeader() {
    const top = document.querySelector('.top-info-bar');
    if (!top) return;
    top.querySelector('span:nth-child(1)').textContent = 'Division: ' + (divList[currentIndex] || '');
    top.querySelector('span:nth-child(2)').textContent = 'District: ' + (distList[currentIndex] || '');
    top.querySelector('span:nth-child(3)').textContent = 'Work Id: ' + (workList[currentIndex] || '');
    top.querySelector('span:nth-child(4)').textContent = 'Status: ' + (statList[currentIndex] || '');
  }
 
  function hideUnusedCameras(){
    boxes.forEach((b,i)=> b.style.display = i < camCount ? 'block' : 'none');
  }
 
function getLiveUrl(i, type){
  const base = streamList[i] || '';
 
  // RTMP → NO quality variants (return as-is)
  if (base.startsWith('rtmp://')) {
    return base;
  }
 
  // RTSP → keep existing logic (UNCHANGED)
  if (type === 'sub') return base.replace(/\.264$/, '_third.264');
  if (type === 'third') return base.replace(/\.264$/, '_fourth.264');
  return base;
}
 
 
  function setAllLiveStreams(){
    for(let i=0;i<camCount;i++){
      const rtsp = getLiveUrl(i, currentStream);
      const url = '${safeBaseUrl}/api/settings/live?rtspUrl=' + encodeURIComponent(rtsp);
      const camId = 'cam' + String(i+1).padStart(2,'0');
      const img = document.getElementById(camId);
      if (img) {
        img.src = url;
        img.onerror = function(){ this.style.background = '#000'; console.warn('image load error', camId, url); };
      }
    }
    hideUnusedCameras();
    updateHeader();
  }
 
  function getActiveChannel() {
  return currentIndex + 1;
}
 
 
function setSingleLiveStream(index){
const isRTMP = (streamList[index] || '').startsWith('rtmp://');
resBtns.forEach(b => {
  b.style.pointerEvents = isRTMP ? 'none' : 'auto';
  b.style.opacity = isRTMP ? '0.5' : '1';
});
 
  const rtsp = getLiveUrl(index, currentStream);
  const url = '${safeBaseUrl}/api/settings/live?rtspUrl=' + encodeURIComponent(rtsp);
  const img = document.getElementById('cam01');
  if (img) img.src = url;
 
  updateHeader();
}
 
// ✅ INITIAL LOAD (THIS WAS MISSING)
setSingleLiveStream(currentIndex);
 
 
  // layout selector: currently only 1 implemented
  if (layoutSelect) layoutSelect.value = '1';
 
  // slider handlers
  if (slideNext) slideNext.onclick = () => {
  currentIndex = (currentIndex + 1) % camCount;
  setSingleLiveStream(currentIndex);
};
 
if (slidePrev) slidePrev.onclick = () => {
  currentIndex = (currentIndex - 1 + camCount) % camCount;
  setSingleLiveStream(currentIndex);
};
 
  // quality buttons
  resBtns.forEach(btn=>{
    btn.onclick = function(){
      if (isPlayback){ alert('Stop playback to change quality'); return; }
      resBtns.forEach(b=>b.classList.remove('active'));
      this.classList.add('active');
      currentStream = this.dataset.stream || 'main';
     if (layoutSelect && layoutSelect.value === '1') {
  setSingleLiveStream(currentIndex);
}
else {
        setAllLiveStreams();
      }
    };
  });
 
  // playback controls (keep unchanged behaviour)
  const pbModal = document.getElementById('playbackPopup');
  const pbBtn = document.getElementById('pbBtn');
  const pbCancel = document.getElementById('pbCancel');
  const pbOk = document.getElementById('pbOk');
 
  const pbDate = document.getElementById('pbDate');
const pbFrom = document.getElementById('pbFrom');
const pbTo   = document.getElementById('pbTo');
function initPlaybackDefaults() {
  const now = new Date();
 
  // yyyy-mm-dd
  const today = now.toISOString().split('T')[0];
 
  // hh:mm
  const hh = String(now.getHours()).padStart(2, '0');
  const mm = String(now.getMinutes()).padStart(2, '0');
 
  // 🔥 ESCAPED TEMPLATE STRING
  const currentTime = hh + ':' + mm;
 
  // 🔒 Fix date
  pbDate.value = today;
  pbDate.max = today;
 
  // 🔒 Time limits
  pbFrom.max = currentTime;
  pbTo.max = currentTime;
 
  // FROM = 10 minutes ago
  const past = new Date(now.getTime() - 10 * 60000);
  pbFrom.value =
    String(past.getHours()).padStart(2, '0') + ':' +
    String(past.getMinutes()).padStart(2, '0');
 
  // TO = now
  pbTo.value = currentTime;
}
 
 
 
if (pbFrom) {
  pbFrom.onchange = function () {
    if (!pbFrom.value) return;
 
    const now = new Date();
    const fromDT = new Date(pbDate.value + 'T' + pbFrom.value + ':00');
 
    // ❌ No future FROM
    if (fromDT > now) {
      alert('From time cannot be in the future');
      pbFrom.value = '';
      return;
    }
 
    // TO = FROM + 5 mins (or now)
    const toDT = new Date(fromDT.getTime() + 5 * 60000);
    const finalTo = toDT > now ? now : toDT;
 
    pbTo.value =
      String(finalTo.getHours()).padStart(2, '0') + ':' +
      String(finalTo.getMinutes()).padStart(2, '0');
  };
}
 
 
 
// ================= DATE & TIME RESTRICTIONS =================
 
if (pbOk) pbOk.onclick = function () {
 
  const dt = pbDate.value;
  const fromVal = pbFrom.value;
  const toVal = pbTo.value;
 
  if (!dt || !fromVal || !toVal) {
    alert('Select Date and Time');
    return;
  }
 
  const now = new Date();
  const fromDT = new Date(dt + 'T' + fromVal + ':00');
  const toDT   = new Date(dt + 'T' + toVal   + ':00');
 
  if (fromDT > now) {
    alert('From time cannot be in the future');
    return;
  }
 
  if (toDT > now) {
    alert('To time cannot be in the future');
    return;
  }
 
  if (toDT < fromDT) {
    alert('To time must be after From time');
    return;
  }
 
  // ✅ FORMAT UNCHANGED
  const dtFmt = dt.replace(/-/g, '');
  const from = fromVal.replace(/:/g, '') + '00';
  const to   = toVal.replace(/:/g, '')   + '00';
 
if (layoutSelect && layoutSelect.value === '1') {
  setPlaybackForChannel(getActiveChannel(), dtFmt, from, to);
} else {
  for (let i = 1; i <= camCount; i++) {
    setPlaybackForChannel(i, dtFmt, from, to);
  }
}
 
 
  isPlayback = true;
  setStatus('Playback');
  pbModal && pbModal.classList.add('hide');
  resBtns.forEach(b => b.style.pointerEvents = 'none');
 
  if (closeBtn) {
    closeBtn.innerText = 'Back';
    window.__restoreLiveFromPlayback = handlePlaybackClose;
  }
};
 
 
 
 
  if (pbBtn) pbBtn.onclick = () => {
  initPlaybackDefaults();
  pbModal && pbModal.classList.remove('hide');
};
 
  if (pbCancel) pbCancel.onclick = ()=> pbModal && pbModal.classList.add('hide');
 
 
 
  function handlePlaybackClose(){
 
  // ✅ Delete temp playback file
  if (currentPlaybackId) {
    fetch('${API_BASE_URL}/api/settings/playback/' + currentPlaybackId, {
      method: 'DELETE'
    }).catch(err => console.warn('Delete playback failed', err));
  }
 
  currentPlaybackId = null;
 
  // ✅ Restore LIVE
  restoreToLive();
 
  // ✅ Change button to Back (navigate)
  if (closeBtn) {
    closeBtn.innerText = 'Back';
  }
}
 
 
function setPlaybackForChannel(ch, dtFmt, from, to) {
 
  if (ch !== currentIndex + 1) return;
 
  const baseStream = streamList[ch - 1] || '';
 
  // 🔹 Resolve correct camera elements
  const camId = String(ch).padStart(2, '0');
  const img = document.getElementById('cam' + camId);
  const video = document.getElementById('cam' + camId + 'Video');
 
  if (!img || !video) return;
 
  /* ================= RTMP PLAYBACK ================= */
  if (baseStream.startsWith('rtmp://')) {
   
    img.style.display = 'none';
    video.style.display = 'block';
 
     const camName = baseStream.split('/').pop();
 
    // Build ISO datetime (NO Z, NO milliseconds)
    const startTime =
      dtFmt.substring(0,4) + '-' +
      dtFmt.substring(4,6) + '-' +
      dtFmt.substring(6,8) + 'T' +
      from.substring(0,2) + ':' +
      from.substring(2,4) + ':00';
 
    const endTime =
      dtFmt.substring(0,4) + '-' +
      dtFmt.substring(4,6) + '-' +
      dtFmt.substring(6,8) + 'T' +
      to.substring(0,2) + ':' +
      to.substring(2,4) + ':00';
 
   
 
 
    fetch('${API_BASE_URL}/api/Settings/playback', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        cameraId: camName,
        startTime: startTime,
        endTime: endTime
      })
    })
    .then(r => {
      if (!r.ok) throw new Error('Playback not found');
      return r.json();
    })
    .then(res => {
      currentPlaybackId = res.data.videoUrl.split('/').pop();
      video.src = res.data.videoUrl;   // ✅ MP4 playback
      video.load();
      video.play();
    })
    .catch(err => {
      console.error('Playback error', err);
      restoreToLive();
      alert('No recording found for selected time');
    });
 
    return;
  }
 
 
  // 🔹 RTSP (UNCHANGED)
  video.pause();
  video.src = '';
  video.style.display = 'none';
  img.style.display = 'block';
 
  const pbURL =
    'rtsp://admin:Admin%401234@43.252.94.42:8554/recording' +
    '?ch=' + ch +
    '&stream=1&start=' + dtFmt + from +
    '&stop=' + dtFmt + to;
 
img.src = "${safeBaseUrl}/api/settings/live?rtspUrl=" + encodeURIComponent(pbURL);
}
 
function restoreToLive(){
  isPlayback = false;
 
  const img = document.getElementById('cam01');
  const video = document.getElementById('cam01Video');
 
  if (video) {
    video.pause();
    video.src = '';
    video.style.display = 'none';
  }
 
  if (img) img.style.display = 'block';
 
  currentStream = 'main';
  setSingleLiveStream(currentIndex);
 
  resBtns.forEach(b => b.style.pointerEvents = 'auto');
  setStatus('Live');
 
    // cleanup global hook
  window.__restoreLiveFromPlayback = null;
}
 
 
  function setStatus(text){
    const dot = document.querySelector('.live-dot');
    if (!dot) return;
    const next = dot.nextSibling;
    if (next) next.textContent = ' ' + text;
  }
 
})(); // IIFE end
    `;
 
      document.body.appendChild(script);
    }, 200); // small delay
  }
 
goBack() {
  const backBtn = document.querySelector('.close-btn') as HTMLElement;
 
  // ✅ Playback active → restore live
  if (backBtn?.innerText === 'Back' && (window as any).__restoreLiveFromPlayback) {
    (window as any).__restoreLiveFromPlayback();
    backBtn.innerText = 'Back';
    return;
  }
 
  // ✅ Normal exit
  sessionStorage.removeItem('liveViewerReloaded');
  // this.router.navigate(['/']);
   this.location.back();
}
 
}