import { getMovieById, getMovies, getMoviesCount, getPopularMovies } from '../dao/movie.js';
import { logger } from '../util/logger.js';

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

export const fetchMovies = (filters, callback) => {
    getMovies(filters, (err, movies) => {
        if (err) {
            logger.error('Movies Error:', err);
            return callback(err);
        }

        const mapped = movies.map((f) => ({
            filmId: f.film_id,
            title: f.title,
            description: f.description,
            releaseYear: f.release_year,
            languageId: f.language_id,
            originalLanguage: f.original_language,
            rentalDuration: f.rental_duration,
            rentalRate: f.rental_rate,
            length: f.length,
            replacementCost: f.replacement_cost,
            rating: f.rating,
            specialFeatures: f.special_features,
            lastUpdate: f.last_update,
            category: f.category,
        }));

        callback(null, mapped);
    });
};

export const fetchMovieCount = (callback) => {
    getMoviesCount((error, count) => {
        if (error) {
            logger.error('Movie Count Error:', error);
            return callback(error);
        }
        callback(null, count);
    });
};

export const fetchMovieById = (id, callback) => {
    getMovieById(id, (error, result) => {
        if (error) {
            logger.error('Movie Error:', error);
            return callback(error);
        }

        const mapped = {
            filmId: result.film_id,
            title: result.title,
            description: result.description,
            releaseYear: result.release_year,
            languageId: result.language_id,
            originalLanguage: result.original_language,
            rentalDuration: result.rental_duration,
            rentalRate: result.rental_rate,
            length: result.length,
            replacementCost: result.replacement_cost,
            rating: result.rating,
            specialFeatures: result.special_features,
            lastUpdate: result.last_update,
            category: result.category,
            coverUrl: result.cover_url,
        };

        callback(null, mapped);
    });
};
