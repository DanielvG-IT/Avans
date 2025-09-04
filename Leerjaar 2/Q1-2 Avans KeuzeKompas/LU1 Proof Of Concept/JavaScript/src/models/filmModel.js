import { dbConnection } from '../data/db';

const getLatestFilms = (limit, cb) => {
    const sql = `
    SELECT film.film_id, title, description, length, rating, release_year
    FROM film
    ORDER BY release_year DESC
    LIMIT ?
  `;
    dbConnection.query(sql, [limit], (err, results) => {
        if (err) return cb(err);
        cb(null, results);
    });
};

const getTopRentals = (limit, cb) => {
    const sql = `
      SELECT f.film_id, f.title, COUNT(r.rental_id) AS total_rentals
      FROM film f
      JOIN inventory i ON i.film_id = f.film_id
      JOIN rental r ON r.inventory_id = i.inventory_id
      GROUP BY f.film_id
      ORDER BY total_rentals DESC
      LIMIT 12;
    `;
    dbConnection.query(sql, [limit], (err, results) => {
        if (err) return cb(err);
        cb(null, results);
    });
};

export { getLatestFilms, getTopRentals };
