import {
    getMovieAvailability,
    getMovieById,
    getMovies,
    getMoviesCount,
    getPopularMovies,
} from '../dao/movie.js';
import { getActorsInMovie } from '../dao/actor.js';
import { logger } from '../util/logger.js';

export const fetchPopularMovies = (limit, callback) => {
    getPopularMovies(limit, (error, movies) => {
        if (error) {
            logger.error('Movies Error:', error);
            return callback(error);
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
    getMovies(filters, (error, movies) => {
        if (error) {
            logger.error('Movies Error:', error);
            return callback(error);
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
            coverUrl: f.cover_url,
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
    getMovieById(id, (error, movie) => {
        if (error) {
            logger.error('Movie Error:', error);
            return callback(error);
        }

        getActorsInMovie(id, (error, actors) => {
            if (error) {
                logger.error('Movie Error:', error);
                return callback(error);
            }

            const actorArray = actors.map((a) => {
                return {
                    firstName: a.first_name,
                    lastName: a.last_name,
                };
            });

            getMovieAvailability(id, (error, stores) => {
                if (error) {
                    logger.error('Movie Error:', error);
                    return callback(error);
                }

                const storeArray = stores.map((s) => {
                    return { storeName: s.store_name, copies: s.copies };
                });

                const mapped = {
                    filmId: movie.film_id,
                    title: movie.title,
                    description: movie.description,
                    releaseYear: movie.release_year,
                    language: movie.language,
                    rentalDuration: movie.rental_duration,
                    rentalRate: movie.rental_rate,
                    length: movie.length,
                    replacementCost: movie.replacement_cost,
                    rating: movie.rating,
                    specialFeatures: movie.special_features,
                    lastUpdate: movie.last_update,
                    category: movie.category,
                    coverUrl: movie.cover_url,
                    actors: actorArray,
                    availability: storeArray,
                };

                callback(null, mapped);
            });
        });
    });
};
