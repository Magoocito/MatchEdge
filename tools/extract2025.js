const fs = require('fs');

// All 2025 matches from screenshots: [date, home, away, hs, as, ho, draw, ao]
const matches = [
  // === Nov 22 ~ ===
  ['2025-12-14','Real Garcilaso','Sporting Cristal',2,0,1.85,3.65,3.75],
  ['2025-12-10','Sporting Cristal','Real Garcilaso',1,0,1.54,3.75,5.00],
  ['2025-12-06','Alianza Lima','Sporting Cristal',3,3,2.00,3.20,3.40],
  ['2025-12-02','Sporting Cristal','Alianza Lima',1,1,2.40,2.95,3.00],
  ['2025-11-23','Comerciantes Unidos','ADT',0,0,4.00,3.10,2.07],
  ['2025-11-23','Alianza Universidad','Juan Pablo II College',3,0,1.99,3.55,3.25],
  ['2025-11-23','Alianza Lima','UTC Cajamarca',3,0,1.13,5.84,12.47],
  ['2025-11-23','Cienciano','Ayacucho',2,1,1.38,4.60,6.00],
  ['2025-11-23','Sport Huancayo','Real Garcilaso',1,2,2.15,3.30,2.87],
  ['2025-11-22','Los Chankas','Universitario',3,1,4.40,3.25,1.76],

  // === Nov 7 ~ ===
  ['2025-11-19','Atlético Grau','Sporting Cristal',1,2,3.35,3.20,2.10],
  ['2025-11-16','Sport Boys','Alianza Atlético',0,1,2.13,2.95,3.25],
  ['2025-11-10','ADT','Los Chankas',1,0,1.58,3.95,5.25],
  ['2025-11-09','Alianza Atlético','Alianza Universidad',2,1,1.43,4.20,7.00],
  ['2025-11-09','UTC Cajamarca','Sport Huancayo',0,3,2.31,3.20,3.00],
  ['2025-11-08','Juan Pablo II College','Atlético Grau',3,1,1.90,3.10,4.20],
  ['2025-11-08','Ayacucho','Comerciantes Unidos',0,3,1.75,3.38,4.00],
  ['2025-11-07','Universitario','Deportivo Garcilaso',0,0,1.12,7.57,20.00],
  ['2025-11-07','Sporting Cristal','Cienciano',2,1,1.60,3.95,4.90],
  ['2025-11-07','Real Garcilaso','Sport Boys',3,0,1.19,5.25,8.75],

  // === Oct 1 ~ ===
  ['2025-11-05','Los Chankas','Alianza Lima',1,2,2.82,3.09,2.42],
  ['2025-11-02','Cienciano','Juan Pablo II College',2,1,1.22,5.75,11.00],
  ['2025-11-02','Sport Boys','UTC Cajamarca',2,2,1.57,3.60,5.00],
  ['2025-11-02','Atlético Grau','Alianza Atlético',1,1,2.70,2.93,2.75],
  ['2025-11-02','Alianza Universidad','Real Garcilaso',2,2,2.75,3.55,2.11],
  ['2025-11-01','Deportivo Garcilaso','ADT',1,0,2.65,3.19,2.55],
  ['2025-11-01','Comerciantes Unidos','Sporting Cristal',1,4,3.75,3.30,2.05],
  ['2025-11-01','Los Chankas','Ayacucho',1,0,2.15,3.20,2.95],
  ['2025-10-31','Alianza Lima','Melgar',2,2,1.83,3.35,3.95],

  // === Sep 27 ~ ===
  ['2025-10-26','Melgar','Sport Huancayo',2,0,1.46,4.30,6.59],
  ['2025-10-26','ADT','Universitario',1,2,3.50,3.20,2.05],
  ['2025-10-26','UTC Cajamarca','Alianza Universidad',3,2,2.01,3.25,3.15],
  ['2025-10-26','Sporting Cristal','Los Chankas',0,1,1.25,7.23,8.20],
  ['2025-10-25','Real Garcilaso','Atlético Grau',1,0,1.35,3.95,7.00],
  ['2025-10-25','Alianza Atlético','Cienciano',0,1,2.05,3.25,3.50],
  ['2025-10-25','Ayacucho','Deportivo Garcilaso',4,2,2.20,3.00,3.00],
  ['2025-10-24','Juan Pablo II College','Comerciantes Unidos',0,1,1.83,3.20,4.25],
  ['2025-10-23','Sporting Cristal','Universitario',0,1,3.10,2.66,2.20],
  ['2025-10-20','Universitario','Ayacucho',2,1,1.11,8.75,33.00],
  ['2025-10-20','Sport Boys','Melgar',1,0,3.41,3.25,2.10],
  ['2025-10-20','Sport Huancayo','Alianza Lima',1,2,2.50,3.10,2.75],
  ['2025-10-19','Deportivo Garcilaso','Sporting Cristal',0,1,2.16,3.45,2.83],
  ['2025-10-19','Atlético Grau','UTC Cajamarca',0,2,1.43,4.10,5.85],
  ['2025-10-18','Cienciano','Real Garcilaso',1,2,2.30,3.10,3.00],
  ['2025-10-18','Comerciantes Unidos','Alianza Atlético',1,0,2.30,2.91,3.11],
  ['2025-10-18','Los Chankas','Juan Pablo II College',1,0,1.63,3.66,3.68],
  ['2025-10-16','Alianza Lima','Sport Boys',3,1,1.44,4.00,6.25],
  ['2025-10-14','Melgar','Alianza Universidad',2,1,1.28,5.00,8.00],
  ['2025-10-13','Juan Pablo II College','Deportivo Garcilaso',0,1,2.00,3.20,3.51],
  ['2025-10-13','UTC Cajamarca','Cienciano',0,2,2.85,3.20,2.22],
  ['2025-10-12','Real Garcilaso','Comerciantes Unidos',1,1,1.45,4.10,6.25],
  ['2025-10-12','Alianza Atlético','Los Chankas',1,0,1.53,4.15,5.60],
  ['2025-10-12','Ayacucho','ADT',2,2,2.55,3.05,2.65],

  // === Sep 18 ~ ===
  ['2025-10-05','Universitario','Juan Pablo II College',3,0,1.16,6.70,14.50],
  ['2025-10-05','Deportivo Garcilaso','Alianza Atlético',2,4,1.55,3.19,5.00],
  ['2025-10-05','Atlético Grau','Melgar',0,0,2.30,2.85,2.80],
  ['2025-10-05','Alianza Universidad','Alianza Lima',2,1,3.75,3.37,2.00],
  ['2025-10-04','Sport Boys','Sport Huancayo',2,1,1.93,3.30,3.40],
  ['2025-10-04','ADT','Sporting Cristal',3,2,2.09,3.10,3.52],
  ['2025-10-04','Los Chankas','Real Garcilaso',2,1,2.70,3.20,2.25],
  ['2025-10-03','Comerciantes Unidos','UTC Cajamarca',2,1,2.30,3.05,3.10],
  ['2025-10-02','Melgar','Cienciano',2,0,1.74,3.60,3.75],
  ['2025-10-01','Alianza Lima','Atlético Grau',2,1,1.37,5.20,6.40],
  ['2025-10-01','Alianza Atlético','Universitario',0,2,5.70,3.40,1.63],

  // === Sep 13 ~ ===
  ['2025-09-30','Real Garcilaso','Deportivo Garcilaso',4,0,1.95,3.30,3.20],
  ['2025-09-30','Juan Pablo II College','ADT',1,1,2.42,3.00,3.25],
  ['2025-09-30','Sport Huancayo','Alianza Universidad',5,1,1.55,4.05,5.30],
  ['2025-09-29','Sporting Cristal','Ayacucho',3,0,1.18,5.25,8.50],
  ['2025-09-29','UTC Cajamarca','Los Chankas',2,0,2.30,3.48,2.95],
  ['2025-09-28','Cienciano','Alianza Lima',2,1,2.10,3.25,3.50],
  ['2025-09-27','Universitario','Real Garcilaso',3,2,1.36,4.20,7.80],
  ['2025-09-27','Comerciantes Unidos','Melgar',1,1,4.59,3.55,1.66],

  // === Sep 22 ~ ===
  ['2025-09-29','Sporting Cristal','César Vallejo',4,1,1.39,4.33,6.30],
  ['2025-09-28','Alianza Lima','Melgar',1,1,1.67,3.50,5.75],
  ['2025-09-28','Cienciano','Real Garcilaso',1,2,2.60,2.90,2.50],
  ['2025-09-27','ADT','Alianza Atlético',0,1,1.55,3.90,6.00],
  ['2025-09-27','Unión Comercio','Sport Boys',0,2,2.80,3.00,2.40],
  ['2025-09-24','UTC Cajamarca','Carlos Manucci',2,0,1.55,3.79,5.25],
  ['2025-09-23','Melgar','Atlético Grau',0,0,1.38,4.47,7.91],
  ['2025-09-23','Sport Huancayo','Los Chankas',3,1,1.78,3.43,4.06],
  ['2025-09-23','Alianza Atlético','Comerciantes Unidos',1,0,1.49,4.05,6.29],
  ['2025-09-22','César Vallejo','ADT',0,0,2.16,3.23,3.29],

  // === Sep 18 ~ ===
  ['2025-09-22','Universitario','Unión Comercio',1,0,1.09,9.41,22.10],
  ['2025-09-22','Real Garcilaso','Sporting Cristal',1,1,2.04,3.33,3.22],
  ['2025-09-21','Sport Boys','Alianza Lima',0,3,4.44,3.34,1.73],
  ['2025-09-21','Deportivo Garcilaso','Cienciano',1,2,2.06,3.23,3.54],
  ['2025-09-19','Carlos Manucci','Melgar',1,1,5.37,3.62,1.57],
  ['2025-09-19','Los Chankas','UTC Cajamarca',1,1,1.59,3.88,4.69],
  ['2025-09-19','Unión Comercio','Alianza Atlético',1,2,2.46,3.13,2.84],
  ['2025-09-18','Universitario','Sport Boys',3,0,1.27,5.21,10.25],
  ['2025-09-18','Cienciano','Sport Huancayo',3,1,2.01,3.23,3.70],
  ['2025-09-18','Comerciantes Unidos','César Vallejo',2,0,2.36,3.08,2.79],

  // === Sep 13 ~ ===
  ['2025-09-18','Atlético Grau','Alianza Lima',1,0,3.39,2.89,2.17],
  ['2025-09-17','ADT','Real Garcilaso',1,2,1.78,3.60,3.80],
  ['2025-09-17','Sporting Cristal','Deportivo Garcilaso',1,0,1.28,5.25,7.00],
  ['2025-09-15','Melgar','Los Chankas',2,0,1.22,5.13,8.38],
  ['2025-09-15','Alianza Atlético','Universitario',0,3,3.70,3.10,2.10],
  ['2025-09-14','Alianza Lima','Carlos Manucci',1,0,1.17,7.00,13.00],
  ['2025-09-14','Sport Huancayo','Sporting Cristal',1,2,2.60,3.30,2.50],
  ['2025-09-14','Sport Boys','Atlético Grau',0,0,2.50,3.10,2.63],
  ['2025-09-14','Real Garcilaso','Comerciantes Unidos',2,1,1.29,4.75,10.00],
  ['2025-09-13','César Vallejo','Unión Comercio',1,0,1.40,4.33,7.00],

  // === Aug 25 ~ ===
  ['2025-09-13','Deportivo Garcilaso','ADT',1,0,2.40,3.20,2.80],
  ['2025-09-13','UTC Cajamarca','Cienciano',1,2,2.20,3.20,3.10],
  ['2025-09-07','Melgar','Comerciantes Unidos',3,0,1.15,6.50,12.00],
  ['2025-09-01','Unión Comercio','Atlético Grau',1,2,2.62,3.10,2.50],
  ['2025-08-26','Carlos Manucci','Atlético Grau',1,1,2.70,3.10,2.40],
  ['2025-08-26','Unión Comercio','Real Garcilaso',3,3,2.60,2.95,2.60],
  ['2025-08-25','Cienciano','Melgar',3,1,3.23,3.12,2.09],
  ['2025-08-25','Comerciantes Unidos','Deportivo Garcilaso',2,1,3.00,3.17,2.17],
  ['2025-08-25','ADT','Sport Huancayo',2,1,1.59,3.65,4.85],
  ['2025-08-25','Sporting Cristal','UTC Cajamarca',4,0,1.24,5.25,8.85],

  // === Aug 20 ~ ===
  ['2025-08-24','Universitario','César Vallejo',1,0,1.31,4.37,8.45],
  ['2025-08-24','Los Chankas','Alianza Lima',0,1,2.97,3.23,2.16],
  ['2025-08-24','Alianza Atlético','Sport Boys',2,0,1.94,3.32,4.46],
  ['2025-08-21','Melgar','Sporting Cristal',2,0,1.60,3.95,5.02],
  ['2025-08-21','Deportivo Garcilaso','Unión Comercio',3,1,1.40,4.42,6.21],
  ['2025-08-21','Atlético Grau','Los Chankas',1,1,1.53,3.86,5.33],
  ['2025-08-21','Sport Boys','Carlos Manucci',2,6,1.66,3.54,4.65],
  ['2025-08-20','Alianza Lima','Cienciano',3,0,1.27,5.32,10.26],
  ['2025-08-20','Real Garcilaso','Universitario',1,1,2.88,3.00,2.51],
  ['2025-08-20','Sport Huancayo','Comerciantes Unidos',2,2,1.44,4.12,6.03],

  // === Aug 15 ~ ===
  ['2025-08-20','UTC Cajamarca','ADT',1,1,2.94,3.22,2.34],
  ['2025-08-19','César Vallejo','Alianza Atlético',0,0,1.88,3.32,4.06],
  ['2025-08-18','Los Chankas','Carlos Manucci',1,1,1.48,4.00,5.75],
  ['2025-08-17','Sporting Cristal','Alianza Lima',0,0,2.20,3.10,3.00],
  ['2025-08-17','Cienciano','Atlético Grau',1,0,1.78,3.30,4.20],
  ['2025-08-17','Unión Comercio','Sport Huancayo',0,1,2.00,3.10,3.50],
  ['2025-08-16','Universitario','Deportivo Garcilaso',3,1,1.13,6.70,14.00],
  ['2025-08-16','César Vallejo','Sport Boys',2,2,2.00,3.20,3.40],
  ['2025-08-16','ADT','Melgar',1,1,2.40,3.00,3.00],
  ['2025-08-15','Comerciantes Unidos','UTC Cajamarca',1,1,2.25,3.19,3.12],

  // === Aug 5 ~ ===
  ['2025-08-15','Alianza Atlético','Real Garcilaso',1,1,2.14,2.98,3.37],
  ['2025-08-11','Real Garcilaso','César Vallejo',2,1,1.50,3.78,5.72],
  ['2025-08-11','Sport Huancayo','Universitario',1,1,3.06,3.07,2.23],
  ['2025-08-11','Carlos Manucci','Cienciano',1,2,2.10,3.00,3.30],
  ['2025-08-11','Atlético Grau','Sporting Cristal',1,1,2.62,3.30,2.38],
  ['2025-08-10','Alianza Lima','ADT',0,0,1.41,3.93,6.90],
  ['2025-08-10','Deportivo Garcilaso','Alianza Atlético',0,1,1.47,3.86,5.97],
  ['2025-08-10','Sport Boys','Los Chankas',2,1,1.67,3.60,4.50],
  ['2025-08-09','UTC Cajamarca','Unión Comercio',3,1,1.57,3.60,5.25],
  ['2025-08-05','Real Garcilaso','Sport Boys',3,1,1.34,4.23,7.85],

  // === Jul 31 ~ ===
  ['2025-08-05','Alianza Atlético','Sport Huancayo',1,1,1.77,3.33,4.24],
  ['2025-08-04','Universitario','UTC Cajamarca',1,0,1.16,5.42,10.64],
  ['2025-08-04','ADT','Atlético Grau',1,2,1.68,3.45,4.59],
  ['2025-08-04','Unión Comercio','Melgar',2,1,4.44,3.40,1.66],
  ['2025-08-04','Sporting Cristal','Carlos Manucci',4,0,1.18,6.26,10.65],
  ['2025-08-03','César Vallejo','Deportivo Garcilaso',0,2,1.73,3.41,4.38],
  ['2025-08-03','Cienciano','Los Chankas',0,0,1.84,3.53,3.65],
  ['2025-08-03','Comerciantes Unidos','Alianza Lima',1,3,4.92,3.66,1.60],
  ['2025-08-01','UTC Cajamarca','Alianza Atlético',1,1,1.56,3.65,6.16],
  ['2025-07-31','Melgar','Universitario',1,0,1.91,2.99,4.09],

  // === Jul 19 ~ ===
  ['2025-07-27','Melgar','Real Garcilaso',0,2,1.60,3.75,4.75],
  ['2025-07-26','Cienciano','Universitario',1,1,2.25,3.05,2.32],
  ['2025-07-26','Sport Boys','Sporting Cristal',0,2,4.38,3.50,1.65],
  ['2025-07-26','Alianza Universidad','Ayacucho',1,2,1.75,3.50,4.50],
  ['2025-07-25','Deportivo Binacional','UTC Cajamarca',0,0,1.80,3.25,3.73],
  ['2025-07-25','Sport Huancayo','Juan Pablo II College',5,1,1.37,4.60,8.60],
  ['2025-07-20','Juan Pablo II College','Sport Boys',3,0,1.96,3.03,4.08],
  ['2025-07-20','Ayacucho','Atlético Grau',1,2,2.61,3.00,2.88],
  ['2025-07-20','Sporting Cristal','Alianza Universidad',3,0,1.22,5.90,10.74],
  ['2025-07-19','Real Garcilaso','Alianza Lima',2,0,1.80,3.40,4.50],
  ['2025-07-19','ADT','Cienciano',1,0,2.15,3.20,3.00],
  ['2025-07-19','UTC Cajamarca','Melgar',1,2,3.25,3.20,1.95],

  // === Jul 12 ~ ===
  ['2025-07-18','Universitario','Comerciantes Unidos',3,1,1.05,11.00,33.00],
  ['2025-07-18','Deportivo Garcilaso','Los Chankas',3,0,2.08,3.35,3.80],
  ['2025-07-18','Alianza Atlético','Sport Huancayo',0,0,1.59,3.25,4.30],
  ['2025-07-13','Real Garcilaso','Sport Huancayo',3,0,1.96,3.15,4.08],
  ['2025-07-13','Juan Pablo II College','Alianza Universidad',1,0,1.98,3.42,4.13],
  ['2025-07-13','Ayacucho','Cienciano',1,0,3.00,3.36,2.18],
  ['2025-07-13','Sporting Cristal','Atlético Grau',2,1,1.50,3.70,6.05],
  ['2025-07-12','Alianza Atlético','Sport Boys',1,0,1.63,3.50,5.50],
  ['2025-07-12','Universitario','Los Chankas',0,0,1.08,8.75,26.00],
  ['2025-07-12','UTC Cajamarca','Alianza Lima',1,0,6.30,3.70,1.50],
  ['2025-07-12','Deportivo Binacional','Melgar',1,1,5.50,3.78,1.57],
  ['2025-07-11','ADT','Comerciantes Unidos',1,0,1.38,5.00,7.67],

  // === Jul 5 ~ ===
  ['2025-07-06','Deportivo Garcilaso','Universitario',0,1,3.25,3.40,2.15],
  ['2025-07-06','Atlético Grau','Juan Pablo II College',2,0,2.50,3.20,2.60],
  ['2025-07-06','Sport Boys','Real Garcilaso',1,2,1.91,3.45,3.40],
  ['2025-07-05','Alianza Lima','Deportivo Binacional',5,1,1.08,9.40,15.00],
  ['2025-07-05','Alianza Universidad','Alianza Atlético',2,0,2.65,3.00,2.60],
  ['2025-07-05','Cienciano','Sporting Cristal',1,1,1.90,3.53,3.35],
  ['2025-07-05','Comerciantes Unidos','Ayacucho',0,1,1.95,3.20,3.05],
  ['2025-07-04','Los Chankas','ADT',1,1,2.52,3.25,2.60],
  ['2025-07-04','Sport Huancayo','UTC Cajamarca',1,0,1.50,4.18,6.25],
  ['2025-07-02','Juan Pablo II College','Melgar',1,1,4.42,3.10,1.99],

  // === Jun 28 ~ ===
  ['2025-06-29','Real Garcilaso','Alianza Universidad',4,1,1.50,4.33,6.00],
  ['2025-06-29','Alianza Atlético','Atlético Grau',2,1,2.20,3.30,3.70],
  ['2025-06-29','Sporting Cristal','Comerciantes Unidos',2,0,1.18,7.00,13.00],
  ['2025-06-28','Melgar','Alianza Lima',0,1,1.83,3.25,4.50],
  ['2025-06-28','UTC Cajamarca','Sport Boys',1,1,2.20,3.20,3.30],
  ['2025-06-28','ADT','Deportivo Garcilaso',0,0,1.73,3.70,4.75],

  // === Jun 22 ~ ===
  ['2025-06-27','Deportivo Binacional','Sport Huancayo',1,0,3.10,3.50,2.15],
  ['2025-06-27','Juan Pablo II College','Cienciano',2,2,2.10,3.20,3.30],
  ['2025-06-27','Ayacucho','Los Chankas',1,2,1.90,3.50,3.90],
  ['2025-06-25','Atlético Grau','Universitario',0,2,7.00,3.50,1.57],
  ['2025-06-22','Cienciano','Alianza Atlético',3,2,1.57,3.70,6.25],
  ['2025-06-22','Comerciantes Unidos','Juan Pablo II College',3,0,2.20,3.10,3.30],
  ['2025-06-22','Sport Boys','Deportivo Binacional',3,1,1.33,5.00,8.50],
  ['2025-06-21','Deportivo Garcilaso','Ayacucho',2,1,1.45,4.50,7.00],
  ['2025-06-21','Los Chankas','Sporting Cristal',3,1,3.10,3.40,2.20],
  ['2025-06-21','Sport Huancayo','Melgar',2,2,1.91,3.60,3.70],
  ['2025-06-20','Universitario','ADT',5,0,1.27,5.75,7.50],
  ['2025-06-20','Alianza Universidad','UTC Cajamarca',1,2,1.65,3.60,5.50],
  ['2025-06-19','Atlético Grau','Real Garcilaso',0,0,1.70,3.50,5.00],
  ['2025-06-18','Comerciantes Unidos','Alianza Lima',0,1,8.50,5.50,1.29],

  // === Jun 16 ~ ===
  ['2025-06-15','Deportivo Binacional','Alianza Universidad',1,1,2.25,3.60,2.88],
  ['2025-06-15','Ayacucho','Universitario',0,1,5.25,3.90,1.62],
  ['2025-06-15','Sporting Cristal','Deportivo Garcilaso',3,2,1.33,4.75,10.00],
  ['2025-06-14','Alianza Lima','Sport Huancayo',0,0,1.38,4.50,9.00],
  ['2025-06-14','Juan Pablo II College','Los Chankas',3,0,1.83,3.25,5.50],
  ['2025-06-14','Melgar','Sport Boys',2,1,1.25,5.50,10.00],
  ['2025-06-13','Real Garcilaso','Cienciano',0,0,2.50,3.40,2.75],
  ['2025-06-13','UTC Cajamarca','Atlético Grau',0,2,2.88,2.90,2.60],
  ['2025-06-12','Alianza Atlético','Comerciantes Unidos',3,0,1.36,4.50,9.00],

  // === Jun 1 ~ ===
  ['2025-06-01','Los Chankas','Alianza Atlético',1,2,1.95,3.25,3.90],

  // === May 18 ~ ===
  ['2025-05-25','Deportivo Garcilaso','Juan Pablo II College',4,0,1.36,4.75,7.00],
  ['2025-05-25','ADT','Ayacucho',0,1,1.36,5.00,7.50],
  ['2025-05-24','Cienciano','UTC Cajamarca',6,1,1.33,4.75,8.00],
  ['2025-05-24','Atlético Grau','Deportivo Binacional',3,0,1.29,5.50,9.00],
  ['2025-05-24','Comerciantes Unidos','Real Garcilaso',1,2,3.00,3.60,2.20],
  ['2025-05-23','Sport Boys','Alianza Lima',0,1,4.20,3.40,1.90],
  ['2025-05-23','Alianza Universidad','Melgar',1,1,3.30,3.20,2.15],
  ['2025-05-22','Universitario','Sporting Cristal',2,0,1.83,3.40,4.33],
  ['2025-05-20','Cienciano','Universitario',0,0,2.63,3.25,2.55],
  ['2025-05-20','UTC Cajamarca','Sport Boys',2,4,1.59,3.66,5.73],
  ['2025-05-19','Alianza Lima','Alianza Universidad',2,0,1.29,5.00,11.00],
  ['2025-05-19','Deportivo Binacional','Cienciano',1,1,3.40,3.40,2.10],
  ['2025-05-18','Melgar','Atlético Grau',1,1,1.42,4.20,8.00],
  ['2025-05-18','Juan Pablo II College','Universitario',2,0,12.00,6.00,1.20],
  ['2025-05-18','Sport Huancayo','Sport Boys',1,0,1.53,4.20,5.75],
  ['2025-05-17','Real Garcilaso','Los Chankas',3,3,1.55,4.10,5.25],
  ['2025-05-17','Sporting Cristal','ADT',2,1,1.36,4.75,8.00],
  ['2025-05-17','UTC Cajamarca','Comerciantes Unidos',2,0,2.00,3.30,3.80],
  ['2025-05-16','Alianza Atlético','Deportivo Garcilaso',1,0,1.75,3.30,4.75],

  // === May 10 ~ ===
  ['2025-05-12','Alianza Universidad','Sport Huancayo',0,1,2.10,3.50,3.00],
  ['2025-05-11','Universitario','Alianza Atlético',0,1,1.42,4.50,7.00],
  ['2025-05-11','Deportivo Garcilaso','Real Garcilaso',1,3,2.63,2.88,2.90],
  ['2025-05-11','ADT','Juan Pablo II College',2,2,1.33,5.25,7.50],
  ['2025-05-11','Los Chankas','UTC Cajamarca',2,1,1.70,3.50,4.75],
  ['2025-05-10','Cienciano','Melgar',1,2,2.88,3.20,2.50],
  ['2025-05-10','Atlético Grau','Alianza Lima',1,1,5.00,3.50,1.70],
  ['2025-05-10','Ayacucho','Sporting Cristal',1,4,1.90,3.40,3.60],
  ['2025-05-09','Comerciantes Unidos','Deportivo Binacional',1,1,1.75,3.40,4.75],

  // === Apr 29 ~ ===
  ['2025-05-06','Atlético Grau','Sport Huancayo',0,0,1.50,3.77,5.95],
  ['2025-05-05','Melgar','Sport Boys',2,1,1.20,6.00,13.00],
  ['2025-05-05','Sporting Cristal','Alianza Atlético',2,1,1.10,9.00,16.00],
  ['2025-05-04','Comerciantes Unidos','Unión Comercio',3,1,1.70,3.80,4.50],
  ['2025-05-04','Carlos Manucci','Deportivo Garcilaso',1,1,2.15,3.40,3.20],
  ['2025-05-04','Los Chankas','Real Garcilaso',2,0,2.08,3.45,3.25],
  ['2025-05-03','Alianza Lima','UTC Cajamarca',1,0,1.22,4.65,9.67],
  ['2025-05-03','Cienciano','César Vallejo',1,1,1.57,3.46,4.68],
  ['2025-05-03','ADT','Universitario',2,0,2.88,3.07,2.14],
  ['2025-04-29','César Vallejo','Sporting Cristal',2,1,3.75,3.50,1.95],

  // === Apr 21 ~ ===
  ['2025-04-28','Universitario','Comerciantes Unidos',6,0,1.17,7.00,12.00],
  ['2025-04-28','Deportivo Garcilaso','Los Chankas',1,1,1.57,4.00,5.25],
  ['2025-04-28','Melgar','Alianza Lima',1,2,1.94,3.15,3.24],
  ['2025-04-28','Alianza Atlético','ADT',1,2,2.30,2.99,2.70],
  ['2025-04-27','Real Garcilaso','Cienciano',2,0,2.25,3.05,2.72],
  ['2025-04-27','UTC Cajamarca','Atlético Grau',3,2,1.88,3.13,3.48],
  ['2025-04-27','Sport Boys','Unión Comercio',0,0,1.62,3.60,6.00],
  ['2025-04-26','Sport Huancayo','Carlos Manucci',1,0,1.40,4.50,7.00],
  ['2025-04-23','Comerciantes Unidos','Alianza Atlético',0,0,1.98,3.15,3.15],
  ['2025-04-21','Atlético Grau','Melgar',1,2,2.48,3.00,2.48],

  // === Apr 19 ~ ===
  ['2025-04-20','Sport Huancayo','Comerciantes Unidos',4,2,1.36,4.75,8.50],
  ['2025-04-20','UTC Cajamarca','ADT',0,1,2.90,3.10,2.35],
  ['2025-04-19','Real Garcilaso','Ayacucho',3,0,1.45,4.00,7.50],
  ['2025-04-19','Alianza Atlético','Sporting Cristal',1,0,3.30,3.50,2.05],
  ['2025-04-19','Sport Boys','Cienciano',2,2,1.73,3.60,4.75],
  ['2025-04-18','Alianza Lima','Los Chankas',1,0,1.30,6.00,7.50],
  ['2025-04-18','Deportivo Binacional','Universitario',1,3,2.90,3.25,2.35],
  ['2025-04-18','Melgar','Deportivo Garcilaso',1,2,1.53,3.80,6.50],
  ['2025-04-17','Alianza Universidad','Atlético Grau',0,0,2.40,3.10,3.00],

  // === Apr 14 ~ ===
  ['2025-04-14','Deportivo Garcilaso','Alianza Lima',0,1,2.15,3.00,3.60],
  ['2025-04-13','Universitario','Melgar',4,1,1.83,3.20,4.75],
  ['2025-04-13','ADT','Deportivo Binacional',1,1,1.29,5.25,10.00],
  ['2025-04-13','Comerciantes Unidos','Sport Boys',0,4,2.20,3.20,3.25],
  ['2025-04-13','Sporting Cristal','Real Garcilaso',1,0,1.40,4.33,8.00],
  ['2025-04-12','Cienciano','Alianza Universidad',0,1,1.48,4.33,5.75],
  ['2025-04-12','Juan Pablo II College','Alianza Atlético',1,0,2.75,3.30,2.45],
  ['2025-04-11','Los Chankas','Sport Huancayo',1,3,2.35,3.20,3.00],
  ['2025-04-11','Ayacucho','UTC Cajamarca',0,1,1.90,3.50,4.00],

  // === Apr 6 ~ ===
  ['2025-04-06','Alianza Universidad','Comerciantes Unidos',0,2,1.65,3.70,5.25],
  ['2025-04-06','UTC Cajamarca','Sporting Cristal',4,1,2.63,3.80,2.30],
  ['2025-04-06','Sport Boys','Los Chankas',1,1,1.60,3.80,5.50],
  ['2025-04-06','Real Garcilaso','Juan Pablo II College',4,2,1.25,6.00,11.00],
  ['2025-04-05','Alianza Lima','Universitario',1,1,2.30,3.25,3.10],
  ['2025-04-05','Melgar','ADT',4,0,1.48,4.00,7.00],
  ['2025-04-05','Atlético Grau','Cienciano',1,1,1.62,3.70,5.50],
  ['2025-04-04','Sport Huancayo','Deportivo Garcilaso',0,3,2.05,3.20,3.60],
  ['2025-04-04','Deportivo Binacional','Ayacucho',1,1,1.70,4.00,4.33],

  // === Mar 28 ~ ===
  ['2025-03-30','Juan Pablo II College','UTC Cajamarca',4,2,2.10,3.20,3.50],
  ['2025-03-30','Los Chankas','Alianza Universidad',3,1,1.91,3.40,3.70],
  ['2025-03-29','Deportivo Garcilaso','Sport Boys',4,0,1.45,3.90,7.00],
  ['2025-03-29','Sporting Cristal','Deportivo Binacional',5,0,1.11,7.50,17.00],
  ['2025-03-29','ADT','Alianza Lima',3,0,1.48,3.90,6.00],
  ['2025-03-28','Universitario','Sport Huancayo',3,1,1.27,4.75,12.00],
  ['2025-03-28','Alianza Atlético','Real Garcilaso',2,4,1.67,3.30,5.00],
  ['2025-03-27','Ayacucho','Melgar',2,3,5.00,3.70,1.57],
  ['2025-03-27','Comerciantes Unidos','Atlético Grau',2,2,3.10,3.10,2.25],

  // === Mar 8 ~ ===
  ['2025-03-23','Los Chankas','Cienciano',0,0,2.55,3.40,2.60],
  ['2025-03-10','Deportivo Binacional','Juan Pablo II College',2,0,1.30,5.00,10.00],
  ['2025-03-10','Atlético Grau','Los Chankas',2,1,1.38,4.33,8.00],
  ['2025-03-09','Cienciano','Comerciantes Unidos',3,2,1.40,5.00,6.00],
  ['2025-03-09','Sport Boys','Universitario',0,2,6.50,4.10,1.48],
  ['2025-03-08','Melgar','Sporting Cristal',1,0,1.75,3.80,4.00],
  ['2025-03-08','Sport Huancayo','ADT',2,1,2.15,3.20,3.50],
  ['2025-03-08','UTC Cajamarca','Alianza Atlético',0,0,2.20,2.90,3.50],
  ['2025-03-07','Alianza Lima','Ayacucho',2,0,1.22,5.75,12.00],
  ['2025-03-07','Alianza Universidad','Deportivo Garcilaso',0,0,2.20,3.40,3.10],

  // === Mar 1 ~ ===
  ['2025-03-02','Universitario','Alianza Universidad',4,0,1.18,6.25,15.00],
  ['2025-03-02','Ayacucho','Sport Huancayo',1,1,2.63,3.50,2.45],
  ['2025-03-01','Sporting Cristal','Alianza Lima',1,2,1.85,3.40,4.50],
  ['2025-03-01','Alianza Atlético','Deportivo Binacional',4,1,1.42,4.00,8.00],
  ['2025-03-01','Real Garcilaso','UTC Cajamarca',1,1,1.38,4.33,8.50],

  // === Feb 27 ~ ===
  ['2025-02-28','Deportivo Garcilaso','Atlético Grau',2,0,1.73,3.50,5.00],
  ['2025-02-27','ADT','Sport Boys',2,2,1.44,4.50,6.50],

  // === Feb 16 ~ ===
  ['2025-02-24','Deportivo Binacional','Real Garcilaso',1,2,2.25,3.30,3.10],
  ['2025-02-23','Melgar','Alianza Atlético',2,1,1.33,4.75,8.50],
  ['2025-02-23','Alianza Universidad','ADT',2,3,2.20,3.20,3.25],
  ['2025-02-22','Cienciano','Deportivo Garcilaso',2,3,2.45,3.20,2.80],
  ['2025-02-22','Sport Huancayo','Sporting Cristal',1,1,2.50,3.40,2.63],
  ['2025-02-22','Sport Boys','Ayacucho',2,1,1.67,3.80,4.75],
  ['2025-02-22','Comerciantes Unidos','Los Chankas',1,1,2.25,3.20,3.20],
  ['2025-02-21','Alianza Lima','Juan Pablo II College',1,0,1.20,5.50,11.00],

  // === Feb 9 ~ ===
  ['2025-02-17','Ayacucho','Alianza Universidad',3,2,2.60,2.88,2.90],
  ['2025-02-16','Deportivo Garcilaso','Comerciantes Unidos',2,1,1.30,5.25,9.00],
  ['2025-02-16','Juan Pablo II College','Sport Huancayo',0,1,2.80,3.10,2.60],
  ['2025-02-16','Sporting Cristal','Sport Boys',2,1,1.38,4.33,8.00],
  ['2025-02-15','Universitario','Cienciano',3,2,1.25,5.00,15.00],
  ['2025-02-15','Alianza Atlético','Alianza Lima',3,1,5.25,3.40,1.70],
  ['2025-02-15','ADT','Atlético Grau',4,3,1.67,3.70,5.25],
  ['2025-02-15','UTC Cajamarca','Deportivo Binacional',0,4,1.67,3.50,5.50],
  ['2025-02-14','Real Garcilaso','Melgar',0,1,2.80,3.30,2.40],

  // === Feb 7 ~ ===
  ['2025-02-10','Los Chankas','Deportivo Garcilaso',2,2,2.70,3.10,2.70],
  ['2025-02-09','Cienciano','ADT',2,2,2.00,3.50,3.50],
  ['2025-02-09','Comerciantes Unidos','Universitario',1,1,7.00,3.40,1.57],
  ['2025-02-09','Sport Boys','Juan Pablo II College',1,0,1.57,4.10,5.25],
  ['2025-02-09','Alianza Universidad','Sporting Cristal',2,2,4.33,3.40,1.90],
  ['2025-02-08','Alianza Lima','Real Garcilaso',3,0,1.38,4.50,8.00],
  ['2025-02-08','Atlético Grau','Ayacucho',1,0,1.33,5.25,8.00],
  ['2025-02-08','Melgar','UTC Cajamarca',3,0,1.17,7.00,12.00],
  ['2025-02-07','Sport Huancayo','Alianza Atlético',2,1,1.50,4.00,6.50],
];

function norm(s) { return s.trim().toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g,'').replace(/[^a-z0-9]/g,''); }

// Deduplicate
const seen = new Set();
const unique = [];
for (const m of matches) {
  const key = `${m[0]}|${norm(m[1])}|${norm(m[2])}`;
  if (!seen.has(key)) { seen.add(key); unique.push(m); }
}

// Write verified CSV
let csv = 'Date,HomeTeam,HomeForm,AwayTeam,AwayForm,HomeScore,AwayScore,HomeOdds,DrawOdds,AwayOdds\n';
for (const [d,h,a,hs,as,ho,do_,ao] of unique) {
  csv += `${d},${h},,${a},,${hs},${as},${ho},${do_},${ao}\n`;
}
fs.writeFileSync('data/footystats-2025-verified.csv', csv);
console.log(`Extracted ${unique.length} unique matches`);

// Compare with original
const csvLines = fs.readFileSync('data/footystats-results-2025.csv','utf-8').trim().split('\n').slice(1);
const original = [];
for (const line of csvLines) {
  if (!line.trim()) continue;
  const p = line.split(',');
  if (p.length >= 10) original.push({ date: p[0], home: p[1].trim(), away: p[3].trim(), hs: +p[5], as: +p[6], ho: +p[7], draw: +p[8], ao: +p[9] });
}

const vLookup = new Map();
unique.forEach(m => vLookup.set(m[0]+'|'+norm(m[1])+'|'+norm(m[2]), m));

let found=0, missing=0, scoreErr=0, oddsErr=0;
const missingList=[], scoreErrList=[];
original.forEach(m => {
  const key = m.date+'|'+norm(m.home)+'|'+norm(m.away);
  const v = vLookup.get(key);
  if (!v) { missing++; missingList.push(`${m.date}: ${m.home} ${m.hs}-${m.as} ${m.away}`); return; }
  found++;
  if (m.hs !== v[3] || m.as !== v[4]) { scoreErr++; scoreErrList.push(`${m.date}: ${m.home} ${m.hs}-${m.as} ${m.away} | Verified: ${v[3]}-${v[4]}`); }
  if (Math.abs(m.ho-v[5])>0.02||Math.abs(m.draw-v[6])>0.02||Math.abs(m.ao-v[7])>0.02) oddsErr++;
});

const cLookup = new Map();
original.forEach(m => cLookup.set(m.date+'|'+norm(m.home)+'|'+norm(m.away), m));
let extraInVerified=0;
unique.forEach(m => { if (!cLookup.has(m[0]+'|'+norm(m[1])+'|'+norm(m[2]))) extraInVerified++; });

console.log(`\n=== AUDITORÍA 2025: CAPTURAS vs CSV ===`);
console.log(`Capturas: ${unique.length} | CSV original: ${original.length}`);
console.log(`Matched: ${found} | Score errors: ${scoreErr} | Odds errors: ${oddsErr}`);
console.log(`In CSV but NOT in captures: ${missing}`);
console.log(`In captures but NOT in CSV: ${extraInVerified}`);
console.log(`Precisión: ${((found-scoreErr)/found*100).toFixed(1)}%`);

if (scoreErrList.length) { console.log('\n--- SCORE ERRORS ---'); scoreErrList.forEach(s => console.log(' ',s)); }
if (missingList.length > 0 && missingList.length <= 15) { console.log('\n--- MISSING IN CAPTURES ---'); missingList.forEach(s => console.log(' ',s)); }
