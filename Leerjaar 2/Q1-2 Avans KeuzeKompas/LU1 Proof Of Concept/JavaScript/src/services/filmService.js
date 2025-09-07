import { getPopularFilms } from '../dao/film.js';

export const fetchPopularFilms = (limit, callback) => {
    getPopularFilms(limit, (err, films) => {
        if (err) return callback(err);

        const mapped = films.map((f) => ({
            filmId: f.film_id,
            title: f.title,
            rentalCount: f.rental_count,
            category: f.category,
            coverUrl: f.cover_url,
        }));

        callback(null, mapped);
    });
};
