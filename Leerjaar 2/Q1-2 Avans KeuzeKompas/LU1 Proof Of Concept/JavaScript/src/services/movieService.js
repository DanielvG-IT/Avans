import { getPopularMovies } from '../dao/movie.js';

export const fetchPopularMovies = (limit, callback) => {
    getPopularMovies(limit, (err, movies) => {
        if (err) {
            logger.error('Movies Error:', err);
            return callback(err);
        }

        const mapped = movies.map((f) => ({
            filmId: f.film_id,
            title: f.title,
            rentalCount: f.rental_count,
            category: f.category,
            coverUrl: f.cover_url,
        }));

        callback(null, mapped);
    });
};
