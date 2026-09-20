const fs = require('fs');

// ALL matches extracted from FootyStats 2023 screenshots
// Format: [date, home, homeScore, away, awayScore, homeOdds, drawOdds, awayOdds]
const screenshots = [
  // Feb 3 ~
  ['2023-02-04','UTC Cajamarca',3,'Cienciano',0, 2.36,3.14,2.87],
  ['2023-02-04','Universitario',4,'Academia Cantolao',0, 1.43,4.47,7.11],
  ['2023-02-04','Atlético Grau',3,'Melgar',0, 3.60,3.10,2.00],
  ['2023-02-03','Real Garcilaso',0,'Sport Huancayo',3, 2.12,3.04,3.14],
  // Feb 4 ~
  ['2023-02-12','Alianza Atlético',2,'UTC Cajamarca',0, 1.97,3.56,3.56],
  ['2023-02-11','ADT',1,'Deportivo Garcilaso',2, 1.43,3.98,4.83],
  ['2023-02-11','Deportivo Binacional',2,'Atlético Grau',4, 1.79,3.60,3.75],
  ['2023-02-11','Academia Cantolao',1,'César Vallejo',2, 3.38,3.40,2.09],
  ['2023-02-10','Cienciano',2,'Real Garcilaso',0, 1.74,3.39,4.07],
  ['2023-02-07','Deportivo Municipal',0,'Carlos Manucci',3, 2.30,3.00,2.90],
  ['2023-02-06','César Vallejo',2,'Alianza Atlético',2, 1.69,3.54,4.13],
  ['2023-02-06','Sport Boys',2,'Unión Comercio',1, 1.94,3.60,3.49],
  ['2023-02-05','Sporting Cristal',3,'Alianza Lima',0, 2.37,2.65,2.65],
  ['2023-02-04','Deportivo Garcilaso',3,'Deportivo Binacional',0, 2.06,3.25,3.25],
  // Feb 12 ~
  ['2023-02-19','Universitario',3,'Alianza Lima',2, 2.45,3.20,2.60],
  ['2023-02-19','Atlético Grau',1,'ADT',1, 1.67,3.70,4.24],
  ['2023-02-18','Sport Boys',3,'Carlos Manucci',1, 2.30,3.30,2.75],
  ['2023-02-18','Sport Huancayo',0,'Cienciano',2, 2.31,3.60,3.06],
  ['2023-02-17','César Vallejo',3,'Unión Comercio',1, 1.59,4.04,5.10],
  ['2023-02-17','Sporting Cristal',1,'Melgar',0, 2.16,3.34,3.26],
  ['2023-02-13','Melgar',0,'Deportivo Municipal',2, 1.21,5.50,8.80],
  ['2023-02-12','Alianza Lima',2,'Sport Boys',0, 1.20,5.85,9.93],
  ['2023-02-12','Carlos Manucci',1,'Sporting Cristal',1, 4.18,3.85,1.65],
  ['2023-02-12','Unión Comercio',1,'Universitario',0, 3.26,3.35,2.00],
  // Feb 19 ~
  ['2023-03-03','Sport Boys',2,'Deportivo Binacional',2, 2.05,3.00,3.25],
  ['2023-02-27','Academia Cantolao',2,'Real Garcilaso',3, 2.46,3.27,2.68],
  ['2023-02-26','Carlos Manucci',2,'Universitario',0, 3.05,3.27,2.01],
  ['2023-02-26','ADT',3,'Deportivo Municipal',0, 1.87,3.40,3.45],
  ['2023-02-26','Unión Comercio',1,'UTC Cajamarca',0, 2.38,3.00,2.63],
  ['2023-02-25','Deportivo Garcilaso',2,'Atlético Grau',0, 1.81,3.54,4.24],
  ['2023-02-25','Alianza Atlético',4,'Sport Huancayo',2, 2.20,3.10,3.15],
  ['2023-02-22','UTC Cajamarca',1,'Academia Cantolao',0, 1.56,3.62,4.98],
  ['2023-02-20','Deportivo Municipal',2,'Deportivo Binacional',0, 1.86,3.32,3.75],
  ['2023-02-19','Real Garcilaso',2,'Alianza Atlético',1, 1.62,3.75,4.50],
  // Feb 25 ~
  ['2023-03-12','Unión Comercio',2,'Sport Huancayo',0, 1.95,3.20,3.50],
  ['2023-03-11','Alianza Lima',2,'Real Garcilaso',0, 1.30,5.00,9.00],
  ['2023-03-10','ADT',1,'Sport Boys',0, 1.53,4.06,5.19],
  ['2023-03-06','Real Garcilaso',2,'Unión Comercio',1, 1.43,4.13,5.95],
  ['2023-03-06','Deportivo Municipal',0,'Deportivo Garcilaso',3, 2.21,2.97,3.07],
  ['2023-03-05','UTC Cajamarca',0,'Alianza Lima',1, 3.00,3.45,2.34],
  ['2023-03-05','Universitario',3,'Melgar',0, 1.50,3.35,3.05],
  ['2023-03-04','César Vallejo',2,'Carlos Manucci',0, 2.00,3.00,3.55],
  ['2023-03-04','Sporting Cristal',3,'ADT',0, 1.40,4.10,6.50],
  ['2023-03-04','Sport Huancayo',4,'Academia Cantolao',0, 1.36,4.33,7.00],
  // Mar 4 ~
  ['2023-03-19','Sport Huancayo',2,'Alianza Lima',1, 2.55,3.20,2.37],
  ['2023-03-19','Sporting Cristal',2,'Atlético Grau',1, 1.42,3.90,6.25],
  // Mar 12 ~
  ['2023-03-19','César Vallejo',2,'Deportivo Binacional',0, 1.60,3.45,5.00],
  ['2023-03-18','Universitario',3,'ADT',1, 1.33,4.50,8.50],
  ['2023-03-18','Cienciano',3,'Unión Comercio',0, 1.40,4.33,7.00],
  ['2023-03-18','UTC Cajamarca',0,'Melgar',0, 2.75,3.10,2.45],
  ['2023-03-18','Alianza Atlético',3,'Academia Cantolao',0, 1.40,3.70,7.50],
  ['2023-03-17','Sport Boys',1,'Deportivo Garcilaso',3, 2.31,3.20,2.85],
  ['2023-03-13','Deportivo Binacional',1,'Universitario',2, 2.55,3.25,2.75],
  ['2023-03-12','Deportivo Garcilaso',4,'Sporting Cristal',1, 2.70,3.60,2.35],
  ['2023-03-12','Academia Cantolao',1,'Cienciano',0, 2.90,3.20,2.30],
  ['2023-03-12','Atlético Grau',2,'Deportivo Municipal',1, 1.74,3.35,4.25],
  // Mar 19 ~
  ['2023-03-26','Sport Boys',0,'Alianza Atlético',1, 2.35,3.05,2.85],
  ['2023-03-26','ADT',0,'Melgar',0, 2.95,3.05,2.35],
  ['2023-03-26','Atlético Grau',1,'Alianza Lima',2, 3.20,3.20,2.02],
  ['2023-03-26','Deportivo Municipal',2,'Unión Comercio',0, 2.20,3.10,3.10],
  ['2023-03-25','César Vallejo',0,'Sport Huancayo',2, 1.70,3.00,4.40],
  ['2023-03-25','Sporting Cristal',2,'Academia Cantolao',0, 1.14,6.00,17.00],
  ['2023-03-24','Universitario',3,'Cienciano',0, 1.40,4.20,7.00],
  ['2023-03-20','Real Garcilaso',3,'Carlos Manucci',0, 1.49,3.83,5.65],
  ['2023-03-19','Sport Huancayo',2,'Alianza Lima',1, 2.55,3.20,2.37],
  ['2023-03-19','Sporting Cristal',2,'Atlético Grau',1, 1.42,3.90,6.25],
  // Mar 26 ~
  ['2023-04-02','Atlético Grau',3,'Sport Boys',0, 1.55,4.30,5.80],
  ['2023-04-02','Carlos Manucci',0,'Sport Huancayo',0, 2.23,3.45,3.20],
  ['2023-04-02','Unión Comercio',3,'Alianza Atlético',3, 2.05,3.30,3.45],
  ['2023-04-01','Alianza Lima',2,'Cienciano',0, 1.37,5.00,8.40],
  ['2023-04-01','Melgar',1,'Real Garcilaso',1, 1.57,3.75,3.75],
  ['2023-03-31','Deportivo Garcilaso',0,'Universitario',1, 2.59,3.34,2.60],
  ['2023-03-31','ADT',2,'César Vallejo',0, 1.91,3.30,3.50],
  ['2023-03-31','Deportivo Municipal',1,'Sporting Cristal',1, 4.50,3.75,1.62],
  ['2023-03-27','UTC Cajamarca',1,'Real Garcilaso',0, 1.70,3.50,4.33],
  ['2023-03-26','Deportivo Garcilaso',3,'Carlos Manucci',2, 1.50,3.90,5.25],
  // Apr 2 ~
  ['2023-04-10','César Vallejo',3,'Deportivo Garcilaso',0, 1.67,3.60,4.50],
  ['2023-04-10','Academia Cantolao',1,'Unión Comercio',2, 2.50,3.10,2.63],
  ['2023-04-09','Universitario',2,'Atlético Grau',1, 1.48,4.20,6.35],
  ['2023-04-09','Sport Huancayo',1,'Melgar',1, 2.24,3.22,3.00],
  ['2023-04-09','Alianza Atlético',0,'Alianza Lima',1, 3.15,3.13,2.16],
  ['2023-04-08','Cienciano',1,'Carlos Manucci',0, 1.25,5.00,11.00],
  ['2023-04-08','UTC Cajamarca',1,'ADT',0, 2.20,3.13,3.20],
  ['2023-04-08','Sport Boys',0,'Deportivo Municipal',2, 2.18,3.40,3.14],
  ['2023-04-07','Real Garcilaso',2,'Deportivo Binacional',1, 1.58,3.48,4.56],
  ['2023-04-02','Deportivo Binacional',1,'UTC Cajamarca',1, 1.80,3.80,4.30],
  // Apr 14 ~
  ['2023-04-21','UTC Cajamarca',1,'Atlético Grau',1, 1.85,3.50,3.90],
  ['2023-04-16','Carlos Manucci',1,'Alianza Atlético',0, 2.33,3.30,2.59],
  ['2023-04-16','Deportivo Binacional',5,'Sport Huancayo',3, 2.24,3.35,2.67],
  ['2023-04-16','Deportivo Municipal',1,'Universitario',2, 3.05,3.10,2.13],
  ['2023-04-16','ADT',0,'Real Garcilaso',2, 2.09,3.15,3.17],
  ['2023-04-16','Deportivo Garcilaso',2,'UTC Cajamarca',2, 1.75,3.40,3.95],
  ['2023-04-15','Alianza Lima',3,'Academia Cantolao',0, 1.25,5.00,12.00],
  ['2023-04-15','Atlético Grau',1,'César Vallejo',1, 2.13,3.46,3.22],
  ['2023-04-14','Melgar',1,'Cienciano',0, 1.78,3.25,3.95],
  ['2023-04-14','Sporting Cristal',1,'Sport Boys',1, 1.25,5.20,7.80],
  // Apr 22 ~
  ['2023-04-29','Atlético Grau',3,'Real Garcilaso',0, 1.82,3.26,3.53],
  ['2023-04-28','Sporting Cristal',1,'César Vallejo',1, 1.36,3.94,6.70],
  ['2023-04-24','Universitario',2,'Sporting Cristal',0, 2.15,3.25,3.00],
  ['2023-04-24','César Vallejo',1,'Deportivo Municipal',1, 1.60,3.40,5.75],
  ['2023-04-24','Alianza Atlético',3,'Melgar',3, 2.20,3.00,3.25],
  ['2023-04-23','Real Garcilaso',2,'Deportivo Garcilaso',1, 1.69,3.38,3.94],
  ['2023-04-23','Unión Comercio',1,'Alianza Lima',3, 3.92,3.30,1.66],
  ['2023-04-22','Cienciano',1,'Deportivo Binacional',1, 1.57,3.70,5.50],
  ['2023-04-22','Sport Huancayo',1,'ADT',3, 1.53,3.60,6.00],
  ['2023-04-22','Academia Cantolao',0,'Carlos Manucci',2, 2.70,3.10,2.40],
  // May 6 ~
  ['2023-05-12','Melgar',2,'Sport Boys',0, 1.16,6.90,16.50],
  ['2023-05-12','Sport Huancayo',5,'UTC Cajamarca',1, 1.62,3.60,4.45],
  ['2023-05-11','Alianza Lima',2,'César Vallejo',0, 1.33,4.30,5.80],
  ['2023-05-10','Cienciano',5,'Alianza Atlético',2, 1.57,3.85,5.25],
  ['2023-05-08','Unión Comercio',1,'Melgar',1, 2.70,3.20,2.30],
  ['2023-05-07','Alianza Lima',3,'Carlos Manucci',0, 1.28,4.60,9.00],
  ['2023-05-07','César Vallejo',2,'Sport Boys',0, 1.40,4.20,7.00],
  ['2023-05-07','UTC Cajamarca',1,'Sporting Cristal',1, 3.30,3.20,2.00],
  ['2023-05-06','Cienciano',0,'Deportivo Garcilaso',0, 1.73,3.55,4.30],
  ['2023-05-06','Sport Huancayo',0,'Atlético Grau',0, 1.60,3.80,5.00],
  // Apr 29 ~
  ['2023-05-06','Alianza Atlético',1,'ADT',1, 1.80,3.45,3.65],
  ['2023-05-05','Real Garcilaso',1,'Deportivo Municipal',0, 1.62,3.62,4.62],
  ['2023-05-05','Academia Cantolao',1,'Deportivo Binacional',0, 3.13,3.40,2.10],
  ['2023-04-30','Carlos Manucci',2,'Unión Comercio',0, 1.76,3.45,4.00],
  ['2023-04-30','ADT',3,'Cienciano',4, 1.99,3.30,3.30],
  ['2023-04-30','Deportivo Municipal',2,'UTC Cajamarca',0, 1.68,3.45,4.45],
  ['2023-04-30','Deportivo Binacional',4,'Alianza Atlético',2, 1.53,3.60,5.50],
  ['2023-04-29','Melgar',5,'Academia Cantolao',0, 1.20,5.75,13.00],
  ['2023-04-29','Deportivo Garcilaso',1,'Sport Huancayo',1, 1.90,3.50,3.60],
  ['2023-04-29','Sport Boys',0,'Universitario',3, 5.50,3.60,1.53],
  // May 12 ~
  ['2023-05-17','Deportivo Binacional',0,'Sporting Cristal',1, 2.60,3.40,2.60],
  ['2023-05-17','Carlos Manucci',0,'UTC Cajamarca',1, 1.90,3.50,4.00],
  ['2023-05-16','Academia Cantolao',0,'Sport Boys',0, 2.50,3.20,2.60],
  ['2023-05-15','Alianza Lima',2,'Deportivo Municipal',1, 1.38,4.45,6.60],
  ['2023-05-15','Melgar',2,'Deportivo Garcilaso',4, 1.48,4.10,5.50],
  ['2023-05-14','Cienciano',1,'César Vallejo',1, 1.88,3.50,3.90],
  ['2023-05-14','Alianza Atlético',3,'Universitario',1, 2.90,3.60,2.20],
  ['2023-05-13','Carlos Manucci',1,'Atlético Grau',0, 2.15,3.25,2.57],
  ['2023-05-13','Unión Comercio',1,'Sporting Cristal',6, 3.40,3.55,1.90],
  ['2023-05-12','Deportivo Binacional',1,'ADT',0, 1.71,3.60,4.19],
  // May 19 ~
  ['2023-05-26','Academia Cantolao',1,'Deportivo Garcilaso',1, 3.30,3.40,1.94],
  ['2023-05-22','Deportivo Binacional',5,'Unión Comercio',0, 1.40,4.25,6.25],
  ['2023-05-22','Sport Boys',3,'UTC Cajamarca',2, 2.24,3.15,2.87],
  ['2023-05-21','Deportivo Municipal',1,'Sport Huancayo',2, 2.20,3.40,3.10],
  ['2023-05-21','Atlético Grau',4,'Cienciano',0, 1.87,3.60,3.90],
  ['2023-05-20','Sporting Cristal',3,'Real Garcilaso',2, 1.37,4.01,6.12],
  ['2023-05-20','Deportivo Garcilaso',3,'Alianza Atlético',2, 1.40,4.25,7.00],
  ['2023-05-20','ADT',1,'Academia Cantolao',1, 1.30,4.60,7.50],
  ['2023-05-19','Universitario',4,'César Vallejo',0, 1.50,4.10,6.00],
  ['2023-05-19','Melgar',2,'Alianza Lima',1, 2.00,3.30,3.55],
  // May 26 ~
  ['2023-06-01','Sporting Cristal',4,'Cienciano',2, 1.33,4.60,8.00],
  ['2023-05-31','Melgar',2,'César Vallejo',2, 1.39,4.80,8.00],
  ['2023-05-28','Alianza Lima',6,'Deportivo Binacional',1, 1.30,5.05,9.30],
  ['2023-05-28','Carlos Manucci',1,'Melgar',2, 2.78,3.09,2.59],
  ['2023-05-28','Sport Huancayo',1,'Sporting Cristal',1, 2.39,3.34,2.73],
  ['2023-05-28','UTC Cajamarca',1,'Universitario',0, 1.63,3.24,2.31],
  ['2023-05-28','Unión Comercio',4,'ADT',3, 1.57,3.28,2.93],
  ['2023-05-27','Real Garcilaso',2,'Sport Boys',1, 1.31,4.23,7.25],
  ['2023-05-27','Alianza Atlético',3,'Atlético Grau',2, 1.95,3.30,3.25],
  ['2023-05-26','Cienciano',1,'Deportivo Municipal',0, 1.45,4.15,5.75],
  // Jun 2 ~
  ['2023-06-10','Unión Comercio',2,'Atlético Grau',2, 2.25,3.30,2.80],
  ['2023-06-09','Cienciano',0,'Sport Boys',1, 1.25,5.50,9.00],
  ['2023-06-04','César Vallejo',3,'UTC Cajamarca',1, 1.43,4.28,7.40],
  ['2023-06-04','Sport Boys',1,'Sport Huancayo',0, 2.80,3.52,2.34],
  ['2023-06-04','Atlético Grau',3,'Academia Cantolao',0, 1.25,4.80,11.00],
  ['2023-06-03','Deportivo Garcilaso',2,'Unión Comercio',2, 1.43,4.50,6.85],
  ['2023-06-03','Deportivo Binacional',2,'Carlos Manucci',1, 1.28,4.80,6.85],
  ['2023-06-03','Deportivo Municipal',2,'Alianza Atlético',1, 1.75,3.40,4.00],
  ['2023-06-02','Universitario',1,'Real Garcilaso',0, 1.29,4.34,7.43],
  ['2023-06-02','ADT',2,'Alianza Lima',1, 2.38,3.35,2.66],
  // Jun 10 ~
  ['2023-06-22','Cienciano',1,'Universitario',1, 1.78,2.59,6.14],
  ['2023-06-22','Melgar',4,'ADT',0, 1.72,4.19,3.46],
  ['2023-06-22','Academia Cantolao',0,'Sporting Cristal',2, 4.73,3.30,1.68],
  ['2023-06-11','Sport Huancayo',1,'Universitario',3, 2.44,3.20,2.58],
  ['2023-06-11','Real Garcilaso',2,'César Vallejo',0, 1.53,3.70,5.25],
  ['2023-06-11','Alianza Atlético',0,'Sporting Cristal',0, 3.55,3.60,1.97],
  ['2023-06-10','Alianza Lima',3,'Deportivo Garcilaso',2, 1.33,4.80,7.00],
  ['2023-06-10','Carlos Manucci',1,'ADT',1, 1.83,3.50,3.70],
  ['2023-06-10','Melgar',1,'Deportivo Binacional',0, 1.40,3.60,6.50],
  ['2023-06-10','Academia Cantolao',0,'Deportivo Municipal',1, 3.20,3.50,2.00],
  // Jun 23 ~
  ['2023-07-01','Deportivo Municipal',0,'Alianza Lima',1, 5.95,4.70,1.40],
  ['2023-07-01','Deportivo Garcilaso',2,'Melgar',2, 2.50,3.20,2.61],
  ['2023-07-01','ADT',2,'Deportivo Binacional',0, 1.13,9.17,8.04],
  ['2023-06-30','UTC Cajamarca',0,'Sport Huancayo',0, 2.41,3.05,2.41],
  ['2023-06-26','Real Garcilaso',2,'UTC Cajamarca',1, 1.45,4.40,6.30],
  ['2023-06-25','Unión Comercio',1,'Deportivo Municipal',3, 1.83,3.35,3.95],
  ['2023-06-25','Alianza Atlético',1,'Sport Boys',2, 1.63,3.70,4.90],
  ['2023-06-24','Carlos Manucci',1,'Deportivo Garcilaso',2, 2.01,3.25,3.20],
  ['2023-06-24','Sport Huancayo',2,'César Vallejo',0, 1.58,3.70,4.60],
  ['2023-06-23','Alianza Lima',2,'Atlético Grau',0, 1.35,4.70,7.25],
  // Jul 2 ~
  ['2023-07-08','Alianza Lima',0,'Sporting Cristal',0, 1.94,3.25,2.97],
  ['2023-07-08','Cienciano',2,'UTC Cajamarca',2, 1.91,3.31,3.00],
  ['2023-07-08','Alianza Atlético',1,'César Vallejo',0, 2.17,3.20,3.05],
  ['2023-07-07','Academia Cantolao',1,'Universitario',4, 8.00,4.33,1.36],
  ['2023-07-07','Melgar',3,'Atlético Grau',0, 1.40,4.20,7.50],
  ['2023-07-04','Sport Boys',0,'Academia Cantolao',1, 1.47,3.90,5.50],
  ['2023-07-03','Universitario',2,'Alianza Atlético',0, 1.26,4.55,5.99],
  ['2023-07-03','César Vallejo',0,'Cienciano',3, 1.87,3.35,3.05],
  ['2023-07-02','Atlético Grau',0,'Carlos Manucci',2, 1.50,3.95,5.55],
  ['2023-07-02','Sporting Cristal',3,'Unión Comercio',0, 1.31,4.80,7.90],
  // Jul 9 ~
  ['2023-07-16','Atlético Grau',2,'Deportivo Binacional',1, 1.33,5.25,8.70],
  ['2023-07-15','César Vallejo',2,'Academia Cantolao',1, 1.30,4.60,8.50],
  ['2023-07-15','Sporting Cristal',3,'Carlos Manucci',2, 1.18,6.00,9.50],
  ['2023-07-15','Deportivo Garcilaso',1,'ADT',2, 1.61,4.10,4.89],
  ['2023-07-14','Universitario',2,'Unión Comercio',0, 1.18,5.95,11.00],
  ['2023-07-14','UTC Cajamarca',1,'Alianza Atlético',0, 1.77,3.50,4.00],
  ['2023-07-10','Sport Huancayo',3,'Real Garcilaso',1, 1.73,3.32,3.83],
  ['2023-07-09','Deportivo Binacional',1,'Deportivo Garcilaso',5, 1.97,3.35,2.84],
  ['2023-07-09','Carlos Manucci',0,'Deportivo Municipal',0, 1.29,5.20,9.60],
  ['2023-07-09','Unión Comercio',0,'Sport Boys',1, 2.00,3.25,3.25],
  // Jul 16 ~
  ['2023-07-23','Melgar',1,'Sporting Cristal',1, 2.10,3.40,3.10],
  ['2023-07-23','Carlos Manucci',0,'Sport Boys',1, 2.25,3.10,2.90],
  ['2023-07-23','Academia Cantolao',2,'UTC Cajamarca',0, 2.40,3.20,2.73],
  ['2023-07-22','Alianza Lima',0,'Universitario',0, 1.92,3.20,3.45],
  ['2023-07-22','Alianza Atlético',2,'Real Garcilaso',0, 2.02,3.09,3.11],
  ['2023-07-21','Cienciano',1,'Sport Huancayo',0, 2.15,3.10,2.95],
  ['2023-07-21','Unión Comercio',1,'César Vallejo',4, 2.58,3.38,2.68],
  ['2023-07-17','Deportivo Municipal',0,'Melgar',1, 4.33,3.56,1.83],
  ['2023-07-16','Real Garcilaso',0,'Cienciano',0, 2.18,3.35,2.63],
  ['2023-07-16','Sport Boys',1,'Alianza Lima',0, 8.50,4.75,1.29],
  // Jul 23 ~
  ['2023-07-31','Universitario',3,'Carlos Manucci',0, 1.23,5.56,9.44],
  ['2023-07-31','Sport Huancayo',2,'Alianza Atlético',0, 1.32,4.60,5.40],
  ['2023-07-31','Sport Boys',0,'Melgar',2, 3.30,3.30,2.00],
  ['2023-07-30','Atlético Grau',1,'Deportivo Garcilaso',1, 1.82,3.14,3.43],
  ['2023-07-29','César Vallejo',1,'Alianza Lima',1, 3.30,3.10,2.15],
  ['2023-07-29','UTC Cajamarca',1,'Unión Comercio',1, 1.47,4.05,5.20],
  ['2023-07-28','Real Garcilaso',3,'Academia Cantolao',0, 1.23,4.77,8.74],
  ['2023-07-27','Sporting Cristal',5,'Deportivo Binacional',0, 1.21,7.35,10.34],
  ['2023-07-24','Deportivo Binacional',4,'Deportivo Municipal',1, 1.50,4.20,6.00],
  ['2023-07-23','ADT',1,'Atlético Grau',0, 1.70,3.60,4.33],
  // Aug 1 ~
  ['2023-08-07','Deportivo Garcilaso',5,'Deportivo Municipal',2, 1.28,5.00,8.00],
  ['2023-08-07','Academia Cantolao',1,'Sport Huancayo',1, 3.55,3.10,1.62],
  ['2023-08-06','Melgar',0,'Universitario',1, 2.05,3.10,3.51],
  ['2023-08-06','ADT',1,'Sporting Cristal',1, 2.45,3.20,2.67],
  ['2023-08-06','Carlos Manucci',2,'César Vallejo',1, 2.75,3.10,2.39],
  ['2023-08-06','Alianza Atlético',2,'Cienciano',0, 2.25,3.10,3.10],
  ['2023-08-05','Alianza Lima',1,'UTC Cajamarca',0, 1.30,4.70,9.00],
  ['2023-08-05','Deportivo Binacional',4,'Sport Boys',0, 1.55,4.15,4.15],
  ['2023-08-04','Unión Comercio',2,'Real Garcilaso',1, 2.24,3.07,3.22],
  ['2023-08-01','Deportivo Municipal',1,'ADT',2, 2.25,3.30,3.15],
  // Aug 11 ~
  ['2023-08-15','Unión Comercio',2,'Cienciano',0, 2.40,3.25,2.65],
  ['2023-08-14','Deportivo Municipal',0,'Atlético Grau',0, 2.11,3.00,2.83],
  ['2023-08-13','Real Garcilaso',1,'Alianza Lima',1, 2.88,3.10,2.13],
  ['2023-08-13','Sporting Cristal',3,'Deportivo Garcilaso',0, 1.36,4.45,6.30],
  ['2023-08-12','Universitario',1,'Deportivo Binacional',0, 1.17,6.90,15.00],
  ['2023-08-12','Cienciano',1,'Academia Cantolao',0, 1.24,4.84,5.80],
  ['2023-08-12','Sport Boys',1,'ADT',1, 3.00,2.37,2.47],
  ['2023-08-11','César Vallejo',1,'Melgar',2, 2.45,3.10,2.50],
  ['2023-08-11','UTC Cajamarca',2,'Carlos Manucci',0, 1.68,3.80,4.85],
  ['2023-08-11','Sport Huancayo',1,'Unión Comercio',1, 1.29,5.25,10.00],
  // Aug 15 ~
  ['2023-08-19','Real Garcilaso',1,'Melgar',1, 2.91,3.31,2.11],
  ['2023-08-19','Alianza Atlético',2,'Unión Comercio',2, 1.87,3.40,3.76],
  ['2023-08-17','Atlético Grau',2,'Sporting Cristal',3, 2.75,3.05,2.40],
  ['2023-08-17','Deportivo Binacional',1,'César Vallejo',0, 2.00,3.05,3.65],
  ['2023-08-16','Alianza Lima',1,'Sport Huancayo',0, 1.51,3.80,5.50],
  ['2023-08-16','Deportivo Garcilaso',3,'Sport Boys',0, 1.33,4.75,8.55],
  ['2023-08-16','Carlos Manucci',0,'Real Garcilaso',1, 2.03,3.18,3.21],
  ['2023-08-16','ADT',2,'Universitario',0, 3.20,3.25,2.25],
  ['2023-08-15','Melgar',1,'UTC Cajamarca',1, 1.25,5.03,5.42],
  ['2023-08-15','Academia Cantolao',1,'Alianza Atlético',0, 2.45,3.25,2.65],
  // Aug 19 ~
  ['2023-08-26','Deportivo Binacional',2,'Real Garcilaso',0, 1.80,3.60,4.00],
  ['2023-08-26','Carlos Manucci',2,'Cienciano',0, 2.15,3.15,3.10],
  ['2023-08-26','Unión Comercio',2,'Academia Cantolao',1, 1.60,3.70,5.00],
  ['2023-08-21','César Vallejo',1,'ADT',1, 1.96,3.20,2.98],
  ['2023-08-21','UTC Cajamarca',3,'Deportivo Binacional',2, 1.73,3.50,4.20],
  ['2023-08-21','Sport Boys',1,'Atlético Grau',1, 2.05,3.10,3.10],
  ['2023-08-20','Cienciano',0,'Alianza Lima',1, 2.29,3.30,2.64],
  ['2023-08-20','Sporting Cristal',1,'Deportivo Municipal',1, 1.18,5.49,9.08],
  ['2023-08-19','Universitario',1,'Deportivo Garcilaso',1, 1.27,5.00,8.35],
  ['2023-08-19','Sport Huancayo',3,'Carlos Manucci',1, 1.57,3.70,5.33],
  // Aug 26 ~
  ['2023-09-03','Deportivo Municipal',0,'César Vallejo',3, 2.90,3.75,2.15],
  ['2023-09-02','Deportivo Binacional',2,'Cienciano',2, 1.91,3.50,3.80],
  ['2023-09-01','Melgar',4,'Alianza Atlético',0, 1.25,5.50,10.00],
  ['2023-09-01','Carlos Manucci',1,'Academia Cantolao',2, 1.53,3.75,5.50],
  ['2023-09-01','Atlético Grau',4,'UTC Cajamarca',2, 1.53,4.00,5.75],
  ['2023-09-10','Universitario',1,'Deportivo Municipal',0, 1.23,5.85,10.50],
  ['2023-09-10','Sport Boys',1,'Sporting Cristal',1, 3.80,4.60,1.70],
  ['2023-09-09','Cienciano',0,'Melgar',1, 2.38,3.35,2.82],
  ['2023-09-09','Academia Cantolao',0,'Alianza Lima',2, 7.80,4.65,1.36],
  ['2023-09-04','César Vallejo',5,'Atlético Grau',0, 1.71,3.33,4.45],
  // Sep 4 ~
  ['2023-10-01','Alianza Atlético',2,'Deportivo Garcilaso',0, 2.20,3.50,3.10],
  ['2023-09-30','Cienciano',2,'Atlético Grau',0, 1.63,3.44,4.27],
  ['2023-09-29','Real Garcilaso',4,'Sporting Cristal',1, 2.46,3.16,2.40],
  ['2023-09-29','Sport Huancayo',2,'Deportivo Municipal',0, 1.14,5.10,14.18],
  ['2023-09-29','UTC Cajamarca',1,'Sport Boys',1, 1.64,3.46,4.16],
  ['2023-09-28','Alianza Lima',0,'Melgar',0, 1.68,3.23,4.23],
  ['2023-09-28','César Vallejo',0,'Universitario',1, 2.66,3.12,2.25],
  ['2023-09-28','Unión Comercio',2,'Deportivo Binacional',2, 2.05,3.15,2.99],
  ['2023-09-25','Sporting Cristal',1,'UTC Cajamarca',1, 1.30,4.11,7.74],
  ['2023-09-25','Atlético Grau',0,'Sport Huancayo',0, 1.67,3.26,4.24],
  // Sep 16 ~
  ['2023-09-20','Universitario',3,'Sport Boys',0, 1.24,4.88,9.95],
  ['2023-09-20','Cienciano',2,'ADT',2, 1.83,3.28,3.85],
  ['2023-09-20','UTC Cajamarca',5,'Deportivo Municipal',1, 1.37,4.06,6.05],
  ['2023-09-20','Unión Comercio',0,'Carlos Manucci',0, 1.97,3.20,3.47],
  ['2023-09-19','Academia Cantolao',0,'Melgar',1, 6.53,4.18,1.37],
  ['2023-09-19','Alianza Atlético',1,'Deportivo Binacional',0, 1.62,3.56,4.52],
  ['2023-09-17','Alianza Lima',3,'Unión Comercio',1, 1.22,6.00,11.00],
  ['2023-09-17','ADT',2,'Sport Huancayo',1, 2.10,3.14,3.43],
  ['2023-09-16','Sporting Cristal',0,'Universitario',0, 2.20,3.50,3.00],
  ['2023-09-16','Deportivo Garcilaso',2,'Real Garcilaso',1, 1.91,3.60,3.50],
  // Sep 21 ~
  ['2023-09-25','Deportivo Municipal',2,'Real Garcilaso',0, 3.16,2.99,2.04],
  ['2023-09-24','Deportivo Garcilaso',0,'Cienciano',3, 2.00,3.27,3.00],
  ['2023-09-24','Carlos Manucci',1,'Alianza Lima',2, 3.97,3.26,1.72],
  ['2023-09-24','Melgar',3,'Unión Comercio',1, 1.24,4.38,9.59],
  ['2023-09-24','Sport Boys',0,'César Vallejo',2, 2.62,2.99,2.36],
  ['2023-09-24','ADT',1,'Alianza Atlético',0, 1.51,3.52,5.13],
  ['2023-09-23','Deportivo Binacional',1,'Academia Cantolao',0, 1.50,3.10,5.13],
  ['2023-09-21','César Vallejo',0,'Sporting Cristal',1, 2.88,3.10,2.27],
  ['2023-09-21','Real Garcilaso',0,'Atlético Grau',0, 1.65,3.55,4.50],
  ['2023-09-21','Sport Huancayo',2,'Deportivo Garcilaso',1, 1.75,3.43,4.03],
  // Oct 1 ~
  ['2023-10-08','ADT',3,'Unión Comercio',0, 1.40,4.00,8.00],
  ['2023-10-08','Atlético Grau',1,'Alianza Atlético',1, 1.80,3.20,4.50],
  ['2023-10-07','Deportivo Garcilaso',0,'Academia Cantolao',0, 1.29,5.00,10.00],
  ['2023-10-07','Deportivo Municipal',4,'Cienciano',1, 3.75,3.25,2.00],
  ['2023-10-03','Deportivo Binacional',1,'Alianza Lima',2, 2.35,3.35,2.95],
  ['2023-10-03','Melgar',2,'Carlos Manucci',1, 1.11,7.00,23.00],
  ['2023-10-03','Sport Boys',2,'Real Garcilaso',1, 2.20,3.20,3.20],
  ['2023-10-03','Sporting Cristal',2,'Sport Huancayo',0, 1.57,4.00,5.00],
  ['2023-10-02','Universitario',3,'UTC Cajamarca',0, 1.50,3.50,6.50],
  ['2023-10-01','Academia Cantolao',0,'ADT',1, 4.47,3.62,1.64],
  // Oct 20 ~
  ['2023-10-28','César Vallejo',3,'Real Garcilaso',1, 2.13,3.23,3.44],
  ['2023-10-23','Academia Cantolao',0,'Atlético Grau',4, 2.90,3.60,2.10],
  ['2023-10-22','Cienciano',1,'Sporting Cristal',0, 2.50,3.10,2.80],
  ['2023-10-22','Carlos Manucci',3,'Deportivo Binacional',2, 1.85,3.20,4.20],
  ['2023-10-22','Sport Huancayo',1,'Sport Boys',0, 1.60,4.00,4.75],
  ['2023-10-22','Unión Comercio',1,'Deportivo Garcilaso',2, 2.00,3.30,3.30],
  ['2023-10-21','Real Garcilaso',1,'Universitario',1, 2.80,3.10,2.40],
  ['2023-10-21','Alianza Lima',0,'ADT',0, 1.45,3.80,7.00],
  ['2023-10-21','Alianza Atlético',1,'Deportivo Municipal',2, 1.44,4.75,5.75],
  ['2023-10-20','UTC Cajamarca',1,'César Vallejo',1, 2.15,3.40,3.10],
  // Oct 28 ~
  ['2023-11-08','Alianza Lima',0,'Universitario',2, 2.14,2.96,3.00],
  ['2023-11-04','Universitario',1,'Alianza Lima',1, 2.10,3.30,3.50],
  ['2023-10-29','ADT',0,'Carlos Manucci',0, 1.67,3.40,4.60],
  ['2023-10-29','Sporting Cristal',3,'Alianza Atlético',0, 1.30,4.62,8.00],
  ['2023-10-29','Universitario',2,'Sport Huancayo',0, 1.29,4.57,8.45],
  ['2023-10-29','Deportivo Binacional',1,'Melgar',2, 2.63,3.15,2.42],
  ['2023-10-29','Deportivo Garcilaso',0,'Alianza Lima',1, 2.57,3.15,2.48],
  ['2023-10-29','Atlético Grau',0,'Unión Comercio',1, 1.85,3.50,3.55],
  ['2023-10-28','Deportivo Municipal',1,'Academia Cantolao',2, 1.34,5.01,7.37],
  ['2023-10-28','Sport Boys',2,'Cienciano',1, 2.37,3.35,2.57],
];

// Read CSV
const csv = fs.readFileSync('data/footystats-results-2023.csv','utf8').split('\n').slice(1);
const csvMatches = csv.filter(l=>l.trim()).map(l=>{
  const p=l.split(',');
  return {d:p[0].trim(),h:p[1].trim(),a:p[3].trim(),hs:parseInt(p[5]),as:parseInt(p[6]),ho:parseFloat(p[7]),do_:parseFloat(p[8]),ao:parseFloat(p[9])};
});

function norm(s){return s.toLowerCase().trim().replace(/á/g,'a').replace(/é/g,'e').replace(/í/g,'i').replace(/ó/g,'o').replace(/ú/g,'u').replace(/ñ/g,'n').replace(/\s+/g,' ')}

let exact=0,fuzzy=0,scoreOk=0,scoreErr=0,notFound=0,oddsErr=0;
const scoreDiffs=[],oddsDiffs=[],notFoundList=[];

for(const [d,h,hs,a,as,ho,do_,ao] of screenshots){
  // Exact match
  let m=csvMatches.find(c=>c.d===d&&norm(c.h)===norm(h)&&norm(c.a)===norm(a));
  let type='EXACT';
  if(!m){
    // Fuzzy: same date, first 4 chars of team names
    m=csvMatches.find(c=>{
      if(c.d!==d) return false;
      const nh=norm(c.h),na=norm(c.a),fh=norm(h),fa=norm(a);
      // Check if names overlap significantly
      const hMatch = nh===fh || nh.includes(fh) || fh.includes(nh) || nh.substring(0,4)===fh.substring(0,4);
      const aMatch = na===fa || na.includes(fa) || fa.includes(na) || na.substring(0,4)===fa.substring(0,4);
      return hMatch && aMatch;
    });
    if(m) type='FUZZY';
  }
  if(!m){
    notFound++;
    notFoundList.push(`${d} ${h} ${hs}-${as} ${a}`);
    continue;
  }
  if(type==='EXACT') exact++; else fuzzy++;
  
  // Score comparison
  if(m.hs===hs&&m.as===as) scoreOk++; 
  else scoreDiffs.push({d,h,hs,a,as,csvHs:m.hs,csvAs:m.as,type});
  
  // Odds comparison (if scores match)
  if(m.hs===hs&&m.as===as){
    const hoDiff=Math.abs(m.ho-ho);
    const doDiff=Math.abs(m.do_-do_);
    const aoDiff=Math.abs(m.ao-ao);
    if(hoDiff>0.15||doDiff>0.15||aoDiff>0.15){
      oddsErr++;
      oddsDiffs.push({d,h,a,hs,as,csvOdds:`${m.ho}/${m.do_}/${m.ao}`,ssOdds:`${ho}/${do_}/${ao}`});
    }
  }
}

console.log(`\n========================================`);
console.log(`  AUDITORÍA COMPLETA: FootyStats 2023`);
console.log(`  Imágenes vs CSV`);
console.log(`========================================\n`);
console.log(`Partidos en imágenes: ${screenshots.length}`);
console.log(`Partidos en CSV: ${csvMatches.length}`);
console.log(`\n--- MATCHING ---`);
console.log(`Exacto: ${exact}/${screenshots.length} (${Math.round(exact*100/screenshots.length)}%)`);
console.log(`Fuzzy: ${fuzzy}/${screenshots.length} (${Math.round(fuzzy*100/screenshots.length)}%)`);
console.log(`Total encontrado: ${exact+fuzzy}/${screenshots.length} (${Math.round((exact+fuzzy)*100/screenshots.length)}%)`);
console.log(`No encontrado: ${notFound}/${screenshots.length}`);
console.log(`\n--- SCORES (solo partidos encontrados) ---`);
console.log(`Correctos: ${scoreOk}/${exact+fuzzy} (${Math.round(scoreOk*100/(exact+fuzzy))}%)`);
console.log(`Incorrectos: ${scoreDiffs.length}/${exact+fuzzy}`);
console.log(`\n--- ODDS (solo partidos con score correcto) ---`);
console.log(`Correctos: ${scoreOk-oddsErr}/${scoreOk}`);
console.log(`Incorrectos: ${oddsErr}/${scoreOk}`);

if(scoreDiffs.length>0){
  console.log(`\n--- SCORES DISPARES ---`);
  for(const s of scoreDiffs){
    console.log(`  ${s.d} | ${s.h} ${s.hs}-${s.as} ${s.a} | CSV: ${s.csvHs}-${s.csvAs} | ${s.type}`);
  }
}
if(oddsDiffs.length>0){
  console.log(`\n--- ODDS DISPARES ---`);
  for(const o of oddsDiffs){
    console.log(`  ${o.d} | ${o.h} ${o.hs}-${o.as} ${o.a} | Imagenes: ${o.ssOdds} vs CSV: ${o.csvOdds}`);
  }
}
if(notFoundList.length>0){
  console.log(`\n--- NO ENCONTRADOS ---`);
  for(const n of notFoundList) console.log(`  ${n}`);
}

fs.writeFileSync('audit-2023-complete.json', JSON.stringify({
  total:screenshots.length, exact, fuzzy, notFound, scoreOk, scoreErr:scoreDiffs.length, oddsErr,
  scoreDiffs, oddsDiffs, notFoundList
},null,2));
