const fs = require('fs');

// All 2024 matches extracted from FootyStats screenshots
// [date, home, away, homeScore, awayScore, homeOdds, drawOdds, awayOdds]
const matches = [
  // ===== Jan 26 ~ =====
  ['2024-01-26','Melgar','Real Garcilaso',2,3,1.52,4.10,5.85],
  ['2024-01-26','UTC Cajamarca','Deportivo Garcilaso',1,0,2.14,3.15,3.05],
  ['2024-01-27','Sporting Cristal','ADT',6,2,1.34,4.95,7.90],
  ['2024-01-27','Sport Huancayo','Sport Boys',1,0,1.49,4.30,5.90],
  ['2024-01-27','Cienciano','Comerciantes Unidos',1,0,1.40,4.60,6.25],
  ['2024-01-28','Atlético Grau','Alianza Atlético',1,1,1.72,3.90,4.20],
  ['2024-01-28','Alianza Lima','César Vallejo',2,1,1.50,4.00,5.00],
  ['2024-01-28','Carlos Manucci','Universitario',0,4,3.73,3.35,1.84],

  // ===== Feb 4 ~ =====
  ['2024-02-02','César Vallejo','Melgar',3,2,2.50,3.35,2.65],
  ['2024-02-02','Real Garcilaso','UTC Cajamarca',1,1,1.42,4.40,6.80],
  ['2024-02-03','Universitario','Atlético Grau',1,0,1.28,5.35,9.20],
  ['2024-02-03','Unión Comercio','Carlos Manucci',2,2,1.72,3.75,4.60],
  ['2024-02-03','ADT','Cienciano',1,1,2.28,3.35,2.95],
  ['2024-02-04','Deportivo Garcilaso','Sport Huancayo',0,2,2.08,3.08,2.99],
  ['2024-02-04','Sport Boys','Sporting Cristal',1,3,3.90,3.50,1.88],
  ['2024-02-04','Alianza Atlético','Alianza Lima',0,2,3.45,3.45,2.02],
  ['2024-02-05','Comerciantes Unidos','Los Chankas',3,2,2.08,3.55,3.25],
  ['2024-02-09','Cienciano','Sporting Cristal',2,2,2.44,3.11,2.44],
  ['2024-02-09','UTC Cajamarca','César Vallejo',4,0,2.08,3.17,2.92],
  ['2024-02-10','Alianza Lima','Universitario',0,1,2.29,2.96,2.74],
  ['2024-02-10','Carlos Manucci','Comerciantes Unidos',1,3,1.63,3.37,4.32],
  ['2024-02-10','Melgar','Alianza Atlético',1,0,1.17,5.58,9.76],
  ['2024-02-11','Sport Huancayo','Real Garcilaso',2,0,1.65,3.50,5.00],
  ['2024-02-11','Sport Boys','Deportivo Garcilaso',2,0,2.15,3.25,3.10],
  ['2024-02-11','Los Chankas','ADT',2,0,2.00,3.30,3.40],

  // ===== Feb 12 ~ =====
  ['2024-02-12','Atlético Grau','Unión Comercio',1,0,1.75,3.61,4.19],
  ['2024-02-15','Sporting Cristal','Los Chankas',4,1,1.36,4.20,7.50],
  ['2024-02-16','Alianza Atlético','UTC Cajamarca',2,0,2.10,3.30,3.10],
  ['2024-02-17','ADT','Carlos Manucci',4,0,1.45,4.20,6.50],
  ['2024-02-17','Real Garcilaso','Deportivo Garcilaso',1,0,1.73,3.60,3.60],
  ['2024-02-17','Universitario','Melgar',2,0,1.73,3.40,4.50],
  ['2024-02-18','Cienciano','Sport Boys',3,2,1.53,3.80,5.50],
  ['2024-02-18','Unión Comercio','Alianza Lima',1,3,3.60,3.30,1.95],
  ['2024-02-19','Comerciantes Unidos','Atlético Grau',2,2,1.65,3.60,5.00],
  ['2024-02-19','César Vallejo','Sport Huancayo',1,1,1.73,3.75,4.33],

  // ===== Feb 23 ~ =====
  ['2024-02-23','Sport Huancayo','Alianza Atlético',4,0,1.42,3.83,5.73],
  ['2024-02-24','Sport Boys','Real Garcilaso',3,0,2.11,3.03,2.98],
  ['2024-02-24','UTC Cajamarca','Universitario',0,0,3.15,3.15,1.98],
  ['2024-02-24','Melgar','Unión Comercio',2,1,1.21,4.69,9.85],
  ['2024-02-24','Carlos Manucci','Sporting Cristal',0,4,4.30,3.70,1.65],
  ['2024-02-25','Alianza Lima','Comerciantes Unidos',5,1,1.34,4.21,6.49],
  ['2024-02-25','Atlético Grau','ADT',2,2,1.35,3.99,6.76],
  ['2024-02-25','Deportivo Garcilaso','César Vallejo',2,0,1.79,3.28,3.61],
  ['2024-02-26','Los Chankas','Cienciano',1,1,2.24,3.26,2.95],
  ['2024-02-29','Alianza Atlético','Deportivo Garcilaso',3,0,1.80,3.50,4.33],

  // ===== Mar 1 ~ =====
  ['2024-03-01','ADT','Alianza Lima',2,0,3.25,3.60,2.00],
  ['2024-03-01','Universitario','Sport Huancayo',2,0,1.40,4.00,9.00],
  ['2024-03-02','Sporting Cristal','Atlético Grau',1,0,1.22,4.77,8.85],
  ['2024-03-02','Comerciantes Unidos','Melgar',0,0,3.20,3.20,2.20],
  ['2024-03-02','César Vallejo','Real Garcilaso',2,2,1.75,3.75,4.33],
  ['2024-03-04','Sport Huancayo','Academia Cantolao',4,0,1.36,4.33,7.00],
  ['2024-03-04','Sporting Cristal','ADT',0,0,1.40,4.10,6.50],
  ['2024-03-04','César Vallejo','Carlos Manucci',1,0,1.80,3.50,3.60],
  ['2024-03-05','Universitario','Melgar',2,1,2.36,3.35,3.05],
  ['2024-03-05','UTC Cajamarca','Alianza Lima',3,0,3.00,3.45,2.34],
  ['2024-03-06','Real Garcilaso','Unión Comercio',2,0,1.43,4.13,5.95],
  ['2024-03-06','Deportivo Municipal','Deportivo Garcilaso',2,1,2.21,2.97,3.07],
  ['2024-03-08','Deportivo Garcilaso','Universitario',2,2,3.31,3.00,1.97],
  ['2024-03-08','UTC Cajamarca','Comerciantes Unidos',2,3,2.02,3.30,3.55],
  ['2024-03-09','Atlético Grau','Cienciano',1,1,1.98,3.45,3.55],
  ['2024-03-09','Melgar','ADT',3,1,1.55,4.10,5.35],
  ['2024-03-09','Carlos Manucci','Los Chankas',2,1,2.18,3.35,3.15],
  ['2024-03-09','Alianza Lima','Sporting Cristal',1,2,2.38,3.35,2.82],
  ['2024-03-10','ADT','Sport Boys',1,0,1.53,4.06,5.19],
  ['2024-03-11','Alianza Lima','Real Garcilaso',2,0,1.30,5.00,9.00],
  ['2024-03-12','Atlético Grau','Deportivo Municipal',1,1,1.74,3.35,4.20],
  ['2024-03-12','Unión Comercio','Sport Huancayo',1,1,1.95,3.20,3.50],
  ['2024-03-12','Deportivo Garcilaso','Sporting Cristal',4,4,2.70,3.50,2.35],
  ['2024-03-12','Academia Cantolao','Cienciano',1,0,2.90,3.40,2.30],
  ['2024-03-13','Deportivo Binacional','Universitario',1,2,2.55,3.25,2.75],
  ['2024-03-13','Cienciano','Alianza Lima',2,1,2.05,3.18,2.96],
  ['2024-03-13','Sporting Cristal','Melgar',1,2,1.57,3.51,4.54],

  // ===== Mar 14 ~ =====
  ['2024-03-14','Alianza Atlético','César Vallejo',1,1,2.38,3.20,2.88],
  ['2024-03-14','Comerciantes Unidos','Sport Huancayo',3,2,3.01,3.01,2.10],
  ['2024-03-14','Carlos Manucci','Sport Boys',1,0,2.10,3.01,3.01],
  ['2024-03-15','Los Chankas','Atlético Grau',0,1,1.56,3.51,4.65],
  ['2024-03-16','ADT','UTC Cajamarca',2,1,1.72,3.25,3.99],
  ['2024-03-28','Deportivo Garcilaso','Comerciantes Unidos',1,2,1.62,3.75,5.50],
  ['2024-03-28','Alianza Lima','Los Chankas',3,0,1.31,4.26,6.99],
  ['2024-03-29','Real Garcilaso','Unión Comercio',1,0,1.50,4.00,7.00],
  ['2024-03-30','Melgar','Cienciano',2,0,1.67,4.00,4.75],
  ['2024-03-30','César Vallejo','Universitario',0,0,3.75,3.43,1.95],
  ['2024-03-30','Atlético Grau','Carlos Manucci',3,0,1.56,3.60,4.52],
  ['2024-03-31','Sport Boys','Alianza Atlético',0,0,1.75,4.00,4.20],
  ['2024-03-31','Sport Huancayo','ADT',0,2,2.10,3.75,2.80],
  ['2024-03-31','UTC Cajamarca','Sporting Cristal',1,2,4.33,3.60,1.80],

  // ===== Apr 6 ~ =====
  ['2024-04-05','Universitario','Alianza Atlético',1,0,1.17,5.27,10.39],
  ['2024-04-05','Atlético Grau','Sport Boys',0,0,1.53,4.00,5.50],
  ['2024-04-06','Cienciano','UTC Cajamarca',1,1,1.35,4.02,6.60],
  ['2024-04-06','Unión Comercio','César Vallejo',2,2,2.18,3.11,2.78],
  ['2024-04-06','Los Chankas','Melgar',2,2,3.12,3.02,2.05],
  ['2024-04-07','ADT','Deportivo Garcilaso',2,2,1.65,3.43,4.15],
  ['2024-04-07','Sporting Cristal','Sport Huancayo',4,0,1.33,4.50,9.00],
  ['2024-04-08','Comerciantes Unidos','Real Garcilaso',0,1,2.09,3.07,2.97],
  ['2024-04-12','Sport Huancayo','Cienciano',1,2,2.00,3.40,3.60],
  ['2024-04-13','Alianza Atlético','Unión Comercio',2,0,1.73,3.50,5.00],
  ['2024-04-13','Melgar','Carlos Manucci',2,0,1.21,4.77,9.41],
  ['2024-04-13','Sport Boys','Universitario',1,2,6.00,3.75,1.57],
  ['2024-04-14','Alianza Lima','Atlético Grau',2,0,1.31,4.02,7.68],
  ['2024-04-14','UTC Cajamarca','Los Chankas',2,0,1.91,3.50,3.75],
  ['2024-04-15','Deportivo Garcilaso','Sporting Cristal',2,3,3.75,3.60,1.91],
  ['2024-04-15','Alianza Lima','Sport Boys',3,0,1.25,5.50,11.00],
  ['2024-04-18','Unión Comercio','Universitario',1,2,5.00,4.33,1.53],
  ['2024-04-19','Cienciano','Deportivo Garcilaso',0,2,1.66,3.48,4.01],
  ['2024-04-20','ADT','César Vallejo',1,0,2.00,3.25,3.75],
  ['2024-04-20','Carlos Manucci','UTC Cajamarca',2,0,2.10,3.30,3.20],
  ['2024-04-21','Sporting Cristal','Real Garcilaso',2,0,1.34,4.12,6.68],

  // ===== Apr 21 ~ =====
  ['2024-04-21','Atlético Grau','Melgar',1,2,2.48,3.00,2.48],
  ['2024-04-23','Comerciantes Unidos','Alianza Atlético',0,0,1.98,3.15,3.15],
  ['2024-04-26','Sport Huancayo','Carlos Manucci',0,0,1.40,4.50,7.00],
  ['2024-04-27','Sport Boys','Unión Comercio',0,0,1.62,3.60,6.00],
  ['2024-04-27','UTC Cajamarca','Atlético Grau',3,2,1.88,3.13,3.48],
  ['2024-04-28','Melgar','Alianza Lima',1,2,1.94,3.15,3.24],
  ['2024-04-28','Real Garcilaso','Cienciano',2,0,2.25,3.05,2.72],
  ['2024-04-28','Deportivo Garcilaso','Los Chankas',1,1,1.57,4.00,5.25],
  ['2024-04-28','Universitario','Comerciantes Unidos',6,0,1.17,7.00,12.00],

  // ===== Apr 29 ~ =====
  ['2024-05-03','ADT','Universitario',2,0,2.88,3.07,2.14],
  ['2024-05-03','Cienciano','César Vallejo',1,1,1.57,3.46,4.68],
  ['2024-05-03','Alianza Lima','UTC Cajamarca',1,0,1.22,4.65,9.67],
  ['2024-05-04','Los Chankas','Real Garcilaso',2,0,2.08,3.45,3.25],
  ['2024-05-04','Carlos Manucci','Deportivo Garcilaso',1,1,2.15,3.40,3.20],
  ['2024-05-04','Comerciantes Unidos','Unión Comercio',3,1,1.70,3.80,4.50],
  ['2024-05-05','Sporting Cristal','Alianza Atlético',2,1,1.10,9.00,16.00],
  ['2024-05-05','Melgar','Sport Boys',2,1,1.20,6.00,13.00],
  ['2024-05-05','Atlético Grau','Sport Huancayo',0,0,1.50,3.77,5.95],
  ['2024-05-06','Real Garcilaso','Deportivo Municipal',1,0,1.62,3.62,4.62],
  ['2024-05-06','Academia Cantolao','Deportivo Binacional',3,1,3.13,3.40,2.10],
  ['2024-05-08','Unión Comercio','Melgar',1,1,2.70,3.20,2.30],

  // ===== May 10 ~ =====
  ['2024-05-10','Unión Comercio','ADT',2,2,2.71,3.32,2.51],
  ['2024-05-10','Cienciano','Alianza Atlético',5,2,1.57,3.85,5.25],
  ['2024-05-11','Real Garcilaso','Carlos Manucci',3,0,1.41,4.17,6.30],
  ['2024-05-11','Sport Huancayo','Alianza Lima',0,2,2.40,3.16,2.97],
  ['2024-05-11','César Vallejo','Los Chankas',1,1,1.58,4.00,5.20],
  ['2024-05-12','Deportivo Garcilaso','Atlético Grau',0,0,1.65,3.25,4.50],
  ['2024-05-12','UTC Cajamarca','Melgar',2,6,3.74,3.36,1.98],
  ['2024-05-12','Universitario','Sporting Cristal',4,1,1.95,3.25,3.50],
  ['2024-05-12','Melgar','Sport Boys',1,0,1.16,6.90,16.50],
  ['2024-05-12','Sport Huancayo','UTC Cajamarca',5,1,1.62,3.60,4.45],
  ['2024-05-12','Deportivo Binacional','ADT',0,1,1.71,3.80,4.19],
  ['2024-05-13','Alianza Atlético','Cienciano',0,1,2.10,3.20,3.60],

  // ===== May 18 ~ =====
  ['2024-05-17','Atlético Grau','Real Garcilaso',4,0,1.82,3.38,4.29],
  ['2024-05-17','Deportivo Binacional','Sporting Cristal',0,1,2.60,3.40,2.60],
  ['2024-05-17','Carlos Manucci','UTC Cajamarca',0,1,1.90,3.50,4.00],
  ['2024-05-18','Sporting Cristal','Unión Comercio',5,1,1.17,6.50,11.00],
  ['2024-05-18','Melgar','Sport Huancayo',4,1,1.29,4.70,8.05],
  ['2024-05-19','ADT','Comerciantes Unidos',2,0,1.43,3.85,5.57],
  ['2024-05-19','Los Chankas','Alianza Atlético',2,0,1.64,3.40,4.82],
  ['2024-05-19','Carlos Manucci','César Vallejo',0,0,2.69,3.28,2.55],
  ['2024-05-19','Alianza Lima','Deportivo Garcilaso',3,2,1.28,4.80,8.50],
  ['2024-05-20','Cienciano','Universitario',0,1,2.63,3.25,2.55],
  ['2024-05-20','UTC Cajamarca','Sport Boys',2,4,1.59,3.66,5.73],
  ['2024-05-20','Sporting Cristal','Real Garcilaso',3,2,1.37,4.01,6.12],
  ['2024-05-20','Deportivo Garcilaso','Alianza Atlético',3,2,1.40,4.25,7.00],
  ['2024-05-20','ADT','Academia Cantolao',1,1,1.30,4.60,7.50],

  // ===== May 25 ~ =====
  ['2024-05-24','Real Garcilaso','Alianza Lima',3,0,2.60,3.10,2.60],
  ['2024-05-24','César Vallejo','Atlético Grau',2,1,1.95,3.20,4.00],
  ['2024-05-25','Sport Huancayo','UTC Cajamarca',1,1,1.62,4.00,4.00],
  ['2024-05-25','Comerciantes Unidos','Sporting Cristal',0,1,6.50,4.50,1.45],
  ['2024-05-25','Deportivo Garcilaso','Melgar',1,3,3.50,3.20,2.10],
  ['2024-05-25','Universitario','Los Chankas',4,0,1.10,8.00,21.00],
  ['2024-05-26','Alianza Atlético','Carlos Manucci',0,0,1.62,3.70,5.50],
  ['2024-05-26','Sport Boys','ADT',1,0,2.45,3.20,2.80],
  ['2024-05-27','Unión Comercio','Cienciano',1,2,2.35,3.10,3.10],
  ['2024-05-27','Real Garcilaso','Sport Boys',2,1,1.31,4.23,7.25],
  ['2024-05-27','Alianza Atlético','Atlético Grau',3,2,1.95,3.30,3.25],
  ['2024-05-28','Alianza Lima','Deportivo Binacional',6,1,1.30,5.05,9.30],
  ['2024-05-28','Carlos Manucci','Melgar',1,2,2.78,3.09,2.59],
  ['2024-05-28','Sport Huancayo','Sporting Cristal',1,2,2.39,3.50,2.73],
  ['2024-05-28','UTC Cajamarca','Universitario',1,0,3.04,3.24,2.31],
  ['2024-05-28','Unión Comercio','ADT',4,3,2.36,3.28,2.93],
  ['2024-05-31','Melgar','César Vallejo',2,2,1.39,4.80,8.00],
  ['2024-06-01','Sporting Cristal','Cienciano',4,2,1.33,4.60,8.00],
  ['2024-06-02','Universitario','Real Garcilaso',1,0,1.29,4.34,7.43],
  ['2024-06-02','ADT','Alianza Lima',2,1,2.38,3.35,2.66],
  ['2024-06-03','Deportivo Garcilaso','Unión Comercio',2,2,1.43,4.50,6.85],
  ['2024-06-03','Deportivo Binacional','Carlos Manucci',3,0,1.28,4.80,8.50],
  ['2024-06-03','Deportivo Municipal','Alianza Atlético',2,1,1.75,3.40,4.00],
  ['2024-06-04','César Vallejo','UTC Cajamarca',3,1,1.43,4.28,7.40],
  ['2024-06-04','Sport Boys','Sport Huancayo',1,0,2.80,3.52,2.34],
  ['2024-06-04','Atlético Grau','Academia Cantolao',3,0,1.25,4.80,11.00],
  ['2024-06-09','Cienciano','Sport Boys',0,1,1.25,5.50,9.00],
  ['2024-06-10','Alianza Lima','Deportivo Garcilaso',3,2,1.33,4.80,7.00],
  ['2024-06-10','Carlos Manucci','ADT',1,1,1.83,3.50,3.70],
  ['2024-06-10','Melgar','Deportivo Binacional',2,1,1.40,4.40,6.50],
  ['2024-06-10','Academia Cantolao','Deportivo Municipal',0,1,3.20,3.50,2.00],
  ['2024-06-10','Unión Comercio','Atlético Grau',2,2,2.25,3.30,2.80],
  ['2024-06-11','Sport Huancayo','Universitario',2,1,2.44,3.20,2.58],
  ['2024-06-11','Real Garcilaso','César Vallejo',1,1,1.53,3.70,5.25],
  ['2024-06-11','Alianza Atlético','Sporting Cristal',1,1,3.55,3.60,1.97],
  ['2024-06-22','Cienciano','Universitario',1,1,1.78,2.59,6.14],
  ['2024-06-22','Melgar','ADT',4,0,1.72,4.19,3.46],
  ['2024-06-22','Academia Cantolao','Sporting Cristal',0,2,4.73,3.30,1.68],
  ['2024-06-23','Alianza Lima','Atlético Grau',1,0,1.35,4.70,7.25],
  ['2024-06-24','Carlos Manucci','Deportivo Garcilaso',1,1,2.01,3.25,3.20],
  ['2024-06-24','Sport Huancayo','César Vallejo',1,1,1.58,3.70,4.60],
  ['2024-06-25','Unión Comercio','Deportivo Municipal',1,2,1.83,3.35,3.95],
  ['2024-06-25','Alianza Atlético','Sport Boys',1,2,1.63,3.70,4.90],
  ['2024-06-26','Real Garcilaso','UTC Cajamarca',2,1,1.45,4.40,6.30],

  // ===== Jul 12 ~ =====
  ['2024-07-12','Deportivo Garcilaso','UTC Cajamarca',1,0,1.60,3.75,4.57],
  ['2024-07-12','ADT','Sporting Cristal',3,1,2.44,3.12,2.44],
  ['2024-07-12','Unión Comercio','Los Chankas',1,2,1.91,3.20,3.65],

  // ===== Jul 13 ~ =====
  ['2024-07-13','Comerciantes Unidos','Cienciano',1,2,2.88,3.25,2.38],
  ['2024-07-13','César Vallejo','Alianza Lima',2,3,3.60,3.15,2.08],
  ['2024-07-13','Universitario','Carlos Manucci',6,0,1.11,8.00,20.00],
  ['2024-07-14','Alianza Atlético','Atlético Grau',0,0,2.30,3.10,2.90],
  ['2024-07-14','Sport Boys','Sport Huancayo',2,1,1.83,3.10,4.20],
  ['2024-07-14','Real Garcilaso','Melgar',0,3,3.00,3.20,2.20],

  // ===== Jul 20 ~ =====
  ['2024-07-19','Sport Huancayo','Deportivo Garcilaso',1,0,1.91,3.25,3.40],
  ['2024-07-19','UTC Cajamarca','Real Garcilaso',2,1,2.25,3.00,2.90],
  ['2024-07-20','Melgar','César Vallejo',5,2,1.29,4.78,7.85],
  ['2024-07-20','Los Chankas','Comerciantes Unidos',2,0,1.55,4.00,5.70],
  ['2024-07-21','Cienciano','ADT',0,1,2.13,3.20,3.05],
  ['2024-07-21','Sporting Cristal','Sport Boys',4,0,1.28,4.87,7.90],
  ['2024-07-21','Carlos Manucci','Unión Comercio',0,0,1.78,3.43,3.90],
  ['2024-07-21','Atlético Grau','Universitario',1,1,3.20,3.33,2.01],
  ['2024-07-21','Alianza Lima','Alianza Atlético',2,0,1.18,5.78,12.30],

  // ===== Jul 26 ~ =====
  ['2024-07-25','César Vallejo','UTC Cajamarca',2,0,1.48,4.03,5.62],
  ['2024-07-25','Real Garcilaso','Sport Huancayo',3,0,1.74,3.83,4.10],
  ['2024-07-25','Alianza Atlético','Melgar',3,1,3.79,3.19,1.91],
  ['2024-07-26','Sport Boys','Deportivo Garcilaso',2,0,1.53,3.80,5.25],
  ['2024-07-26','Comerciantes Unidos','Carlos Manucci',2,1,1.73,3.50,4.20],

  // ===== Jul 31 ~ =====
  ['2024-07-31','Carlos Manucci','ADT',3,2,3.56,3.05,2.04],
  ['2024-07-31','Los Chankas','Sporting Cristal',3,3,2.80,3.46,2.19],
  ['2024-07-31','Atlético Grau','Comerciantes Unidos',3,0,1.37,4.42,6.90],

  // ===== Aug 1 ~ =====
  ['2024-08-01','Deportivo Municipal','ADT',1,2,2.25,3.30,3.15],
  ['2024-08-03','Cienciano','Los Chankas',0,0,1.84,3.53,3.65],
  ['2024-08-03','Comerciantes Unidos','Alianza Lima',1,3,4.92,3.66,1.60],
  ['2024-08-03','César Vallejo','Deportivo Garcilaso',0,2,1.73,3.41,4.38],
  ['2024-08-04','Universitario','UTC Cajamarca',1,0,1.16,5.42,10.64],
  ['2024-08-04','ADT','Atlético Grau',1,2,1.68,3.45,4.59],
  ['2024-08-04','Unión Comercio','Melgar',2,1,4.44,3.40,1.66],
  ['2024-08-04','Sporting Cristal','Carlos Manucci',4,0,1.18,6.26,10.65],

  // ===== Aug 5 ~ =====
  ['2024-08-05','Alianza Atlético','Sport Huancayo',1,1,1.77,3.33,4.24],
  ['2024-08-05','Real Garcilaso','Universitario',3,1,1.91,2.99,4.09],
  ['2024-08-05','Melgar','Universitario',1,0,1.91,2.99,4.09],
  ['2024-08-05','Real Garcilaso','Sport Boys',3,1,1.34,4.23,7.85],
  ['2024-08-09','UTC Cajamarca','Unión Comercio',3,1,1.57,3.60,5.25],
  ['2024-08-10','Alianza Lima','ADT',0,0,1.41,3.93,6.90],
  ['2024-08-10','Deportivo Garcilaso','Alianza Atlético',0,1,1.47,3.86,5.97],
  ['2024-08-10','Sport Boys','Los Chankas',2,1,1.67,3.60,4.50],
  ['2024-08-11','Sport Huancayo','Universitario',1,1,3.06,3.07,2.23],
  ['2024-08-11','Carlos Manucci','Cienciano',1,2,2.10,3.00,3.30],
  ['2024-08-11','Atlético Grau','Sporting Cristal',1,1,2.62,3.30,2.38],
  ['2024-08-11','Real Garcilaso','César Vallejo',2,1,1.50,3.78,5.72],

  // ===== Aug 15 ~ =====
  ['2024-08-15','Comerciantes Unidos','UTC Cajamarca',1,0,2.25,3.19,3.12],
  ['2024-08-15','Alianza Atlético','Real Garcilaso',1,1,2.14,2.98,3.37],
  ['2024-08-16','César Vallejo','Sport Boys',2,2,2.00,3.20,3.40],
  ['2024-08-16','ADT','Melgar',1,1,2.40,3.00,3.00],
  ['2024-08-16','Universitario','Deportivo Garcilaso',3,1,1.13,6.70,14.00],
  ['2024-08-17','Los Chankas','Carlos Manucci',1,1,1.48,4.00,5.75],
  ['2024-08-17','Sporting Cristal','Alianza Lima',0,0,2.20,3.10,3.00],
  ['2024-08-17','Cienciano','Atlético Grau',1,0,1.78,3.30,4.20],
  ['2024-08-17','Unión Comercio','Sport Huancayo',0,1,2.00,3.10,3.50],
  ['2024-08-18','César Vallejo','Alianza Atlético',0,0,1.88,3.32,4.06],

  // ===== Aug 20 ~ =====
  ['2024-08-20','Sport Huancayo','Comerciantes Unidos',2,2,1.44,4.12,6.03],
  ['2024-08-20','Real Garcilaso','Universitario',1,1,2.88,3.00,2.51],
  ['2024-08-20','Alianza Lima','Cienciano',3,0,1.27,5.32,10.26],
  ['2024-08-21','Sport Boys','Carlos Manucci',2,6,1.66,3.54,4.65],
  ['2024-08-21','Deportivo Garcilaso','Unión Comercio',3,1,1.40,4.42,6.21],
  ['2024-08-21','Atlético Grau','Los Chankas',1,1,1.53,3.86,5.33],
  ['2024-08-21','Melgar','Sporting Cristal',2,0,1.60,3.95,5.02],
  ['2024-08-24','Los Chankas','Alianza Lima',0,1,2.97,3.23,2.16],
  ['2024-08-24','Alianza Atlético','Sport Boys',2,0,1.94,3.32,4.46],
  ['2024-08-24','Universitario','César Vallejo',1,0,1.31,4.37,8.45],

  // ===== Aug 25 ~ =====
  ['2024-08-25','Sporting Cristal','UTC Cajamarca',4,0,1.24,5.25,8.85],
  ['2024-08-25','ADT','Sport Huancayo',2,1,1.59,3.65,4.85],
  ['2024-08-25','Comerciantes Unidos','Deportivo Garcilaso',2,1,3.00,3.17,2.17],
  ['2024-08-25','Cienciano','Melgar',3,1,3.23,3.12,2.09],
  ['2024-08-26','Unión Comercio','Real Garcilaso',3,3,2.60,2.95,2.60],
  ['2024-08-26','Unión Comercio','Atlético Grau',1,2,2.62,3.10,2.50],
  ['2024-08-26','Carlos Manucci','Atlético Grau',1,1,2.70,3.10,2.40],

  // ===== Sep 1 ~ =====
  ['2024-09-01','Comerciantes Unidos','Universitario',0,2,7.50,4.50,1.42],

  // ===== Sep 13 ~ =====
  ['2024-09-13','César Vallejo','Unión Comercio',1,0,1.40,4.33,7.00],
  ['2024-09-14','Real Garcilaso','Comerciantes Unidos',2,1,1.29,4.75,10.00],
  ['2024-09-14','Sport Boys','Atlético Grau',0,0,2.50,3.10,2.63],
  ['2024-09-14','Alianza Lima','Carlos Manucci',1,0,1.17,7.00,13.00],
  ['2024-09-14','Sport Huancayo','Sporting Cristal',1,2,2.60,3.30,2.50],
  ['2024-09-15','Alianza Atlético','Universitario',0,3,3.70,3.10,2.10],
  ['2024-09-15','Melgar','Los Chankas',2,0,1.22,5.13,8.38],
  ['2024-09-17','ADT','Real Garcilaso',1,2,1.78,3.60,3.80],
  ['2024-09-17','Sporting Cristal','Deportivo Garcilaso',1,0,1.28,5.25,7.00],
  ['2024-09-17','Alianza Lima','Unión Comercio',3,1,1.22,6.00,11.00],
  ['2024-09-17','ADT','Sport Huancayo',2,1,2.10,3.40,3.40],
  ['2024-09-18','Atlético Grau','Alianza Lima',1,0,3.39,2.89,2.17],

  // ===== Sep 18 ~ =====
  ['2024-09-19','Alianza Atlético','Sporting Cristal',1,0,3.50,3.58,1.98],
  ['2024-09-19','Universitario','ADT',2,1,1.28,5.00,8.50],
  ['2024-09-22','Los Chankas','Deportivo Garcilaso',1,0,1.90,3.30,4.20],
  ['2024-09-22','Sporting Cristal','César Vallejo',4,1,1.39,4.33,6.30],
  ['2024-09-22','Alianza Lima','Melgar',1,1,1.67,3.50,5.75],
  ['2024-09-22','Cienciano','Real Garcilaso',1,2,2.50,2.90,2.50],
  ['2024-09-22','ADT','Alianza Atlético',0,1,1.55,3.90,6.00],
  ['2024-09-22','Unión Comercio','Sport Boys',0,2,2.80,3.00,2.40],
  ['2024-09-23','Sport Huancayo','Los Chankas',3,1,1.78,3.43,4.06],
  ['2024-09-23','Melgar','Atlético Grau',0,0,1.38,4.47,7.91],
  ['2024-09-23','UTC Cajamarca','Carlos Manucci',2,0,1.55,3.79,5.25],
  ['2024-09-24','Universitario','Unión Comercio',1,0,1.09,9.41,22.10],
  ['2024-09-24','Real Garcilaso','Sporting Cristal',1,1,2.04,3.33,3.22],
  ['2024-09-24','Sport Boys','Alianza Lima',0,3,4.44,3.34,1.73],
  ['2024-09-24','Deportivo Garcilaso','Cienciano',1,2,2.06,3.23,3.54],
  ['2024-09-24','Carlos Manucci','Melgar',1,1,5.37,3.62,1.57],
  ['2024-09-24','Los Chankas','UTC Cajamarca',1,1,1.59,3.88,4.69],
  ['2024-09-24','Unión Comercio','Alianza Atlético',1,2,2.46,3.13,2.84],
  ['2024-09-18','Universitario','Sport Boys',3,0,1.27,5.21,10.25],
  ['2024-09-18','Cienciano','Sport Huancayo',3,1,2.01,3.23,3.70],
  ['2024-09-18','Comerciantes Unidos','César Vallejo',2,0,2.36,3.08,2.79],

  // ===== Sep 22 ~ =====
  ['2024-09-29','Sporting Cristal','Comerciantes Unidos',4,1,1.28,5.25,7.00],
  ['2024-09-29','Atlético Grau','UTC Cajamarca',4,0,1.62,3.55,5.01],
  ['2024-09-29','Sport Boys','UTC Cajamarca',1,1,1.73,3.28,4.42],
  ['2024-09-29','Unión Comercio','Sporting Cristal',0,12,7.21,4.46,1.32],
  ['2024-09-29','Sport Huancayo','Melgar',2,4,3.43,3.43,1.93],
  ['2024-09-29','Universitario','Cienciano',3,1,1.22,5.00,10.06],
  ['2024-09-29','Comerciantes Unidos','Universitario',0,2,7.50,4.50,1.42],
  ['2024-09-29','Los Chankas','Deportivo Garcilaso',1,0,1.90,3.30,4.20],
  ['2024-09-30','Atlético Grau','UTC Cajamarca',4,0,1.62,3.55,5.01],

  // ===== Oct 18 ~ =====
  ['2024-10-17','Unión Comercio','Comerciantes Unidos',3,2,2.12,3.30,3.08],
  ['2024-10-17','Sport Huancayo','Atlético Grau',1,3,1.90,3.25,4.15],
  ['2024-10-17','Real Garcilaso','Los Chankas',2,1,1.40,4.50,7.25],
  ['2024-10-18','César Vallejo','Unión Comercio',1,0,1.40,4.33,7.00],
  ['2024-10-18','UTC Cajamarca','Alianza Lima',0,1,3.88,3.19,1.80],
  ['2024-10-18','Sport Boys','Melgar',0,2,3.60,2.95,2.20],
  ['2024-10-18','César Vallejo','Cienciano',2,2,1.72,3.60,4.75],
  ['2024-10-19','Alianza Atlético','Sporting Cristal',1,0,3.50,3.58,1.98],
  ['2024-10-19','Universitario','ADT',2,1,1.28,5.00,8.50],
  ['2024-10-18','Sport Boys','UTC Cajamarca',1,1,1.73,3.28,4.42],

  // ===== Oct 23 ~ =====
  ['2024-10-22','ADT','Unión Comercio',2,0,1.34,4.96,7.83],
  ['2024-10-22','Melgar','UTC Cajamarca',1,0,1.20,5.97,13.36],
  ['2024-10-22','Los Chankas','César Vallejo',2,1,1.80,3.49,4.23],
  ['2024-10-22','Alianza Lima','Sport Huancayo',2,1,1.19,6.13,12.96],
  ['2024-10-22','Atlético Grau','Deportivo Garcilaso',2,2,1.67,3.67,4.87],
  ['2024-10-23','Cienciano','Alianza Atlético',3,0,1.55,3.51,5.95],
  ['2024-10-23','Carlos Manucci','Real Garcilaso',4,1,2.56,3.64,2.42],
  ['2024-10-23','Sporting Cristal','Universitario',2,1,2.68,2.58,3.16],
  ['2024-10-24','Comerciantes Unidos','Sport Boys',0,0,2.05,3.30,3.65],
  ['2024-10-23','Academia Cantolao','Atlético Grau',0,4,2.90,3.60,2.10],
  ['2024-10-22','Cienciano','Sporting Cristal',1,0,2.50,3.10,2.80],

  // ===== Oct 28 ~ =====
  ['2024-10-28','Deportivo Garcilaso','Alianza Lima',0,1,2.57,3.15,2.48],
  ['2024-10-28','Sport Boys','Cienciano',2,1,2.37,3.35,2.57],
  ['2024-10-28','César Vallejo','Real Garcilaso',3,1,2.13,3.23,3.44],
  ['2024-10-26','César Vallejo','Carlos Manucci',0,2,1.47,3.63,5.45],
  ['2024-10-26','Deportivo Garcilaso','Alianza Lima',1,2,3.75,3.14,2.03],
  ['2024-10-27','Sport Boys','UTC Cajamarca',1,1,1.73,3.28,4.42],
  ['2024-10-27','Unión Comercio','Sporting Cristal',0,12,7.21,4.46,1.32],
  ['2024-10-27','Sport Huancayo','Melgar',2,4,3.43,3.43,1.93],
  ['2024-10-27','Universitario','Cienciano',3,1,1.22,5.00,10.06],
  ['2024-10-28','Alianza Atlético','Los Chankas',1,0,1.64,3.76,5.15],
  ['2024-10-28','Comerciantes Unidos','ADT',0,0,3.20,3.60,2.10],
  ['2024-10-28','Real Garcilaso','Atlético Grau',0,1,1.63,3.82,5.15],
  ['2024-10-29','Cienciano','Unión Comercio',7,0,1.01,10.75,30.81],

  // ===== Nov 1 ~ =====
  ['2024-11-01','ADT','Sport Boys',1,0,1.44,4.08,7.00],
  ['2024-11-02','Carlos Manucci','Alianza Atlético',2,2,1.44,4.32,6.34],
  ['2024-11-02','Atlético Grau','César Vallejo',3,1,1.90,3.50,3.70],
  ['2024-11-02','UTC Cajamarca','Sport Huancayo',1,1,1.38,4.25,8.05],
  ['2024-11-03','Sporting Cristal','Comerciantes Unidos',3,0,1.11,7.10,15.00],
  ['2024-11-03','Los Chankas','Universitario',0,0,7.20,4.85,1.39],
  ['2024-11-03','Alianza Lima','Real Garcilaso',1,2,1.09,9.50,23.00],
  ['2024-11-03','Melgar','Deportivo Garcilaso',1,1,1.22,6.25,10.00],
];

// Deduplicate
const seen = new Set();
const unique = [];
for (const m of matches) {
  const key = `${m[0]}|${m[1]}|${m[2]}`;
  if (!seen.has(key)) {
    seen.add(key);
    unique.push(m);
  }
}

// Write CSV
let csv = 'Date,HomeTeam,HomeForm,AwayTeam,AwayForm,HomeScore,AwayScore,HomeOdds,DrawOdds,AwayOdds\n';
for (const [d,h,a,hs,as,ho,do_,ao] of unique) {
  csv += `${d},${h},,${a},,${hs},${as},${ho},${do_},${ao}\n`;
}
fs.writeFileSync('data/footystats-2024-verified.csv', csv);
console.log(`Extracted ${unique.length} unique matches from screenshots`);
console.log(`Written to footystats-2024-verified.csv`);

// Quick stats
const teams = {};
unique.forEach(m => {
  teams[m[1]] = (teams[m[1]]||0) + 1;
  teams[m[2]] = (teams[m[2]]||0) + 1;
});
console.log(`\nTeams found: ${Object.keys(teams).length}`);
Object.entries(teams).sort((a,b) => b[1]-a[1]).forEach(([t,c]) => console.log(`  ${t}: ${c}`));
