import WebMap from '@arcgis/core/WebMap';
import MapView from '@arcgis/core/views/MapView';

import "@arcgis/core/assets/esri/themes/light/main.css";
import './style.css';

const map = new WebMap({
  portalItem: {
    id: '38ab49e013e941dabbee9ccdc187815a'
  }
});

const view = new MapView({
  map: map,
  container: 'app'
});

const btn = document.getElementById('btnGo');
btn.onclick = async () => {
  const response = await fetch("http://localhost:3001/");
  debugger;
  const data = await response.json();
  console.log('Clicked on the button', data)
  const features = data.cafes.map((cafe) => ({
    attributes: {
      Naam: cafe.Naam,
      Beschrijving: cafe.Beschrijving
    },
    geometry: {
      type: "point",
      x: cafe.Locatie[1],
      y: cafe.Locatie[0],
      wkid: 4326
    }
  }))
  
  const results = await map.layers.items[0].applyEdits({
    addFeatures: features
  });
  console.log('Applied edits', results)
  
}

view.ui.add(btn, 'top-right')