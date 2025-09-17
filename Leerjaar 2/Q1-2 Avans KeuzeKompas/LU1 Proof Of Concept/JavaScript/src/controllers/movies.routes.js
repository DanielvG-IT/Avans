import { requireStaffAuthApi, requireStaffAuthWeb } from '../middleware/auth.js';
import { fetchStaff } from '../services/authService.js';
import { readMovieById } from '../dao/movie.js';
import { readStores } from '../dao/store.js';
import { logger } from '../util/logger.js';
import express from 'express';
import {
    fetchCategoryNames,
    fetchLanguageNames,
    fetchRatingNames,
} from '../services/filterService.js';
import {
    updateMovieById,
    fetchMovieById,
    fetchMovies,
    addMovie,
} from '../services/movieService.js';

const moviesRouter = express.Router();

// Simple helper to normalize query params into arrays
const normalizeMulti = (val) => {
    if (!val) return [];
    if (Array.isArray(val)) return val;
    if (typeof val === 'string')
        return val
            .split(',')
            .map((s) => s.trim())
            .filter(Boolean);
    return [];
};

// Simple helper to parse ints
const parsePositiveInt = (v, fallback) => {
    const n = parseInt(v, 10);
    return Number.isInteger(n) && n > 0 ? n : fallback;
};

moviesRouter.get('/', (req, res, next) => {
    const page = parsePositiveInt(req.query.page, 1);
    const limit = parsePositiveInt(req.query.limit, 25);
    const search = (req.query.search || '').trim();

    // Sorting (only "field,dir" supported for simplicity)
    let [sortField, sortDir] = (req.query.sort || 'title,asc').split(',');
    sortField = (sortField || 'title').trim();
    sortDir = (sortDir || 'asc').toLowerCase() === 'desc' ? 'desc' : 'asc';

    // TODO: Fix sort (length, releaseYear, rentalRate) actually working but not rendering correctly in the view after

    const categories = normalizeMulti(req.query.category);
    const ratings = normalizeMulti(req.query.rating);

    // Fetch categories and ratings first
    fetchCategoryNames((err, categoryList) => {
        if (err) return next(err);

        fetchRatingNames((err, ratingList) => {
            if (err) return next(err);

            // Now fetch movies
            fetchMovies(
                {
                    page: page - 1, // service expects zero-based
                    limit,
                    search,
                    sortBy: { [sortField]: sortDir },
                    category: categories,
                    rating: ratings,
                },
                (err, result) => {
                    if (err) return next(err);

                    const rows = result.movies || result || [];
                    const total = result.total || rows.length;
                    const totalPages = Math.max(Math.ceil(total / limit), 1);

                    // build encoded base query (without page)
                    const params = new URLSearchParams();
                    if (search) params.set('search', search);
                    if (categories && categories.length)
                        params.set('category', categories.join(','));
                    if (ratings && ratings.length) params.set('rating', ratings.join(','));
                    if (sortField) params.set('sort', `${sortField},${sortDir}`);
                    if (limit && limit !== 25) params.set('limit', String(limit));
                    const baseQuery = params.toString(); // e.g. "search=term&category=Comedy"
                    const pageQueryPrefix = baseQuery ? `?${baseQuery}&page=` : '?page=';

                    // create pagesToShow array (objects: {page: n, active: bool} or {ellipsis: true})
                    const makePagesToShow = (current, total, maxButtons = 7) => {
                        const pages = [];

                        if (total <= maxButtons) {
                            for (let i = 1; i <= total; i++)
                                pages.push({ page: i, active: i === current });
                            return pages;
                        }

                        // always show first
                        pages.push({ page: 1, active: current === 1 });

                        const windowSize = maxButtons - 2; // reserve spots for first and last
                        let start = Math.max(2, current - Math.floor(windowSize / 2));
                        let end = Math.min(total - 1, start + windowSize - 1);

                        // re-calc start if we're near the end
                        start = Math.max(2, end - windowSize + 1);

                        if (start > 2) {
                            pages.push({ ellipsis: true });
                        }

                        for (let i = start; i <= end; i++) {
                            pages.push({ page: i, active: i === current });
                        }

                        if (end < total - 1) {
                            pages.push({ ellipsis: true });
                        }

                        // always show last
                        pages.push({ page: total, active: current === total });

                        return pages;
                    };

                    const pagesToShow = makePagesToShow(page, totalPages, 6);

                    // prev/next page numbers (null if disabled)
                    const prevPage = page > 1 ? page - 1 : null;
                    const nextPage = page < totalPages ? page + 1 : null;

                    res.render('movies/movies', {
                        title: 'Movies',
                        movies: rows,
                        categories: categoryList || [],
                        ratings: ratingList || [],
                        search,
                        category: categories.join(','),
                        rating: ratings.join(','),
                        sort: sortField,
                        sortDir,
                        page,
                        total,
                        totalPages,
                        limit,

                        // pagination helpers for the template:
                        pagesToShow,
                        // use: href="{{pageQueryPrefix}}{{this.page}}"
                        pageQueryPrefix,
                        prevPage,
                        nextPage,
                    });
                }
            );
        });
    });
});

moviesRouter.get('/search', requireStaffAuthApi, (req, res) => {
    const q = (req.query.q || '').trim();
    const page = parsePositiveInt(req.query.page, 0);
    const limit = parsePositiveInt(req.query.limit, 10);
    const sort = req.query.sort || 'title,asc';
    let [sortField, sortDir] = sort.split(',');
    sortField = (sortField || 'title').trim();
    sortDir = (sortDir || 'asc').toLowerCase() === 'desc' ? 'desc' : 'asc';
    const category = normalizeMulti(req.query.category);
    const rating = normalizeMulti(req.query.rating);

    if (!q) return res.json([]);

    // Build filters object for fetchMovies
    const filters = {
        page,
        limit,
        search: q,
        sortBy: { [sortField]: sortDir },
        category,
        rating,
    };

    fetchMovies(filters, (err, result) => {
        if (err) {
            logger.error('Movie search error:', err);
            return res.status(500).json([]);
        }
        // Support both shapes from service: array or { movies, total }
        const rows = Array.isArray(result) ? result : (result && result.movies) || [];
        // return compact objects for the UI (snake_case expected by the client)
        const out = rows.map((f) => ({
            film_id: f.film_id ?? f.filmId,
            title: f.title,
            release_year: f.release_year ?? f.releaseYear,
            rating: f.rating,
            rental_rate: f.rental_rate ?? f.rentalRate,
            rental_duration: f.rental_duration ?? f.rentalDuration,
            length: f.length,
            description: f.description,
        }));
        res.json(out);
    });
});

// Create new movie (same render page & similar logic as edit)
moviesRouter.get('/new', requireStaffAuthWeb, (req, res, next) => {
    fetchRatingNames((ratingErr, ratings) => {
        if (ratingErr) {
            logger.error('New movie: fetchRatingNames error', { ratingErr });
            return next(ratingErr);
        }
        fetchLanguageNames((langErr, languages) => {
            if (langErr) {
                logger.error('New movie: fetchLanguageNames error', { langErr });
                return next(langErr);
            }
            fetchCategoryNames((catErr, categories) => {
                if (catErr) {
                    logger.error('New movie: fetchCategoryNames error', { catErr });
                    return next(catErr);
                }

                // Render empty form for creating a movie
                res.render('movies/createOrEdit', {
                    title: 'Add New Movie',
                    movie: {}, // empty movie for the form
                    languages,
                    ratings,
                    categories,
                    formMethod: 'POST',
                    cancelUrl: '/movies',
                });
            });
        });
    });
});

moviesRouter.post('/new', requireStaffAuthApi, (req, res, next) => {
    console.log('[DEBUG] Entering POST /movies/new', { body: req.body });

    // Collect submitted fields (same names as edit)
    const moviePayload = {
        title: req.body.title,
        description: req.body.description,
        release_year: req.body.release_year,
        language_id: req.body.language_id,
        rental_duration: req.body.rental_duration,
        rental_rate: req.body.rental_rate,
        length: req.body.length,
        replacement_cost: req.body.replacement_cost,
        rating: req.body.rating,
        category: req.body.category,
        special_features: req.body.special_features,
    };

    addMovie(
        moviePayload.title,
        moviePayload.description,
        moviePayload.release_year,
        moviePayload.language_id,
        moviePayload.category,
        moviePayload.rental_duration,
        moviePayload.rental_rate,
        moviePayload.length,
        moviePayload.replacement_cost,
        moviePayload.rating,
        moviePayload.special_features,
        (err, newMovieId) => {
            if (err) {
                logger.error('Create movie: addMovie error', { err });
                // Re-fetch select lists and render the form with the submitted data and an error message
                fetchRatingNames((ratingErr, ratings) => {
                    if (ratingErr) return next(ratingErr);
                    fetchLanguageNames((langErr, languages) => {
                        if (langErr) return next(langErr);
                        fetchCategoryNames((catErr, categories) => {
                            if (catErr) return next(catErr);

                            // Map posted payload to view model shape expected by template
                            const movie = {
                                filmId: undefined,
                                title: moviePayload.title,
                                description: moviePayload.description,
                                releaseYear: moviePayload.release_year,
                                language: (
                                    languages.find(
                                        (l) => String(l.id) === String(moviePayload.language_id)
                                    ) || {}
                                ).name,
                                rentalDuration: moviePayload.rental_duration,
                                rentalRate: moviePayload.rental_rate,
                                length: moviePayload.length,
                                replacementCost: moviePayload.replacement_cost,
                                rating: moviePayload.rating,
                                category: moviePayload.category,
                                specialFeatures: moviePayload.special_features,
                            };

                            return res.status(err.statusCode || 400).render('movies/createOrEdit', {
                                title: 'Add New Movie',
                                movie,
                                languages,
                                ratings,
                                categories,
                                cancelUrl: '/movies',
                                formMethod: 'POST',
                                errorMessage:
                                    err.message ||
                                    'Could not create the movie. Please correct the errors and try again.',
                            });
                        });
                    });
                });
                return; // prevent fallthrough
            }
            logger.info('Create movie: success', { newMovieId });
            return res.redirect(`/movies/${newMovieId}`);
        }
    );
});

// Movie details
moviesRouter.get('/:id', (req, res, next) => {
    const id = parseInt(req.params.id, 10);
    if (!Number.isInteger(id) || id <= 0) {
        return next(new Error('Invalid movie id'));
    }

    fetchMovieById(id, (err, movie) => {
        if (err) return next(err);

        // NEW: return JSON when requested so the client modal can fetch details
        const wantsJson =
            req.xhr ||
            (req.get && /application\/json/.test(req.get('accept') || '')) ||
            String(req.query.format || '').toLowerCase() === 'json';

        if (wantsJson) {
            return res.json({
                film_id: movie.film_id ?? movie.filmId,
                title: movie.title,
                release_year: movie.release_year ?? movie.releaseYear,
                rating: movie.rating,
                rental_rate: movie.rental_rate ?? movie.rentalRate,
                rental_duration: movie.rental_duration ?? movie.rentalDuration,
                length: movie.length,
                description: movie.description,
            });
        }

        res.render('movies/movie', { title: movie.title, movie });
    });
});

moviesRouter.get('/:id/edit', requireStaffAuthWeb, (req, res, next) => {
    const movieId = parseInt(req.params.id, 10);
    if (!Number.isInteger(movieId) || movieId <= 0) {
        logger.warn('Edit movie: invalid movie id', { movieId });
        return next(new Error('Invalid movie id'));
    }

    fetchMovieById(movieId, (err, movie) => {
        if (err) {
            logger.error('Edit movie: fetchMovieById error', { movieId, err });
            return next(err);
        }
        if (!movie) {
            logger.warn('Edit movie: movie not found', { movieId });
            return next(new Error('Movie not found'));
        }

        fetchRatingNames((ratingErr, ratings) => {
            if (ratingErr) {
                logger.error('Edit movie: fetchRatingNames error', { movieId, ratingErr });
                return next(ratingErr);
            }
            fetchLanguageNames((langErr, languages) => {
                if (langErr) {
                    logger.error('Edit movie: fetchLanguageNames error', { movieId, langErr });
                    return next(langErr);
                }

                fetchCategoryNames((catErr, categories) => {
                    if (catErr) {
                        logger.error('Edit movie: fetchCategoryNames error', {
                            movieId,
                            catErr,
                        });
                        return next(catErr);
                    }

                    res.render('movies/createOrEdit', {
                        title: 'Edit Movie',
                        movie,
                        languages,
                        ratings,
                        categories,
                        cancelUrl: '/movies/' + movieId,
                    });
                });
            });
        });
    });
});
moviesRouter.post('/:id/edit', requireStaffAuthApi, (req, res, next) => {
    const movieId = parsePositiveInt(req.params.id, null);
    console.log('[DEBUG] Entering POST /movies/:id/edit', {
        paramsId: req.params.id,
        movieId,
        body: req.body,
    });
    if (!movieId) {
        console.log('[DEBUG] Invalid movie id in POST /movies/:id/edit', {
            paramsId: req.params.id,
        });
        logger.warn('Edit movie: invalid movie id', { movieId: req.params.id });
        return next(new Error('Invalid movie id'));
    }

    console.log(req.body);

    const title = req.body.title;
    const description = req.body.description;
    const releaseYear = req.body.release_year;
    const languageId = req.body.language_id;
    const rentalDuration = req.body.rental_duration;
    const rentalRate = req.body.rental_rate;
    const length = req.body.length;
    const replacementCost = req.body.replacement_cost;
    const rating = req.body.rating;
    const specialFeatures = req.body.special_features;

    console.log('[DEBUG] Calling updateMovieById', { movieId });
    updateMovieById(
        movieId,
        title,
        description,
        releaseYear,
        languageId,
        rentalDuration,
        rentalRate,
        length,
        replacementCost,
        rating,
        specialFeatures,
        (err) => {
            if (err) {
                console.log('[DEBUG] updateMovieById returned error', err);
                logger.error('Edit movie: updateMovie error', { movieId, err });

                let errorMessage;
                fetchMovieById(movieId, (fetchErr, movie) => {
                    console.log('[DEBUG] fetchMovieById callback', { fetchErr, movie });
                    if (fetchErr) {
                        console.log('[DEBUG] fetchMovieById error', fetchErr);
                        logger.error('Edit movie: fetchMovieById error', { movieId, fetchErr });
                        errorMessage = fetchErr;
                    }
                    if (!movie) {
                        console.log('[DEBUG] fetchMovieById returned no movie', { movieId });
                        logger.warn('Edit movie: movie not found', { movieId });
                        errorMessage = 'Movie not found';
                    }

                    fetchRatingNames((ratingErr, ratings) => {
                        console.log('[DEBUG] fetchRatingNames callback', { ratingErr, ratings });
                        if (ratingErr) {
                            console.log('[DEBUG] fetchRatingNames error', ratingErr);
                            logger.error('Edit movie: fetchRatingNames error', {
                                movieId,
                                ratingErr,
                            });
                            errorMessage = ratingErr;
                        }
                        fetchLanguageNames((langErr, languages) => {
                            console.log('[DEBUG] fetchLanguageNames callback', {
                                langErr,
                                languages,
                            });
                            if (langErr) {
                                console.log('[DEBUG] fetchLanguageNames error', langErr);
                                logger.error('Edit movie: fetchLanguageNames error', {
                                    movieId,
                                    langErr,
                                });
                                errorMessage = langErr;
                            }
                            fetchCategoryNames((catErr, categories) => {
                                console.log('[DEBUG] fetchCategoryNames callback', {
                                    catErr,
                                    categories,
                                });
                                if (catErr) {
                                    console.log('[DEBUG] fetchCategoryNames error', catErr);
                                    logger.error('Edit movie: fetchCategoryNames error', {
                                        movieId,
                                        catErr,
                                    });
                                    errorMessage = catErr;
                                }
                                console.log('[DEBUG] Rendering createOrEdit with errorMessage', {
                                    errorMessage,
                                });
                                return res.render('movies/createOrEdit', {
                                    title: 'Edit Movie',
                                    movie,
                                    languages,
                                    ratings,
                                    categories,
                                    errorMessage,
                                    cancelUrl: '/movies/' + movieId,
                                    formMethod: 'POST',
                                });
                            });
                        });
                    });
                });
                return;
            }
            console.log('[DEBUG] updateMovieById callback complete - redirecting', { movieId });
            return res.redirect('/movies/' + movieId);
        }
    );
});

// Staff-only: rent a movie page (mounted under /movies)
moviesRouter.get('/:id/rent', requireStaffAuthWeb, (req, res, next) => {
    const movieId = parsePositiveInt(req.params.id, null);
    if (!movieId) return next(new Error('Invalid movie ID'));

    readStores((storeError, stores) => {
        if (storeError) {
            logger.error('Store Error:', storeError);
            return next(storeError);
        }

        fetchStaff(req.user?.userId, (staffErr, staff) => {
            if (staffErr) {
                logger.warn(
                    'Staff fetch error for rent page; proceeding with req.user fallback',
                    staffErr
                );
            }

            if (!staff) {
                logger.error('Staff fetch returned no results; cannot proceed');
                return next(new Error('Staff fetch returned no results; cannot proceed'));
            }

            // proceed even if staff is null; template falls back to user.id
            readMovieById(movieId, (filmErr, film) => {
                if (filmErr) {
                    logger.error('Film fetch error for rent page:', filmErr);
                    return next(filmErr);
                }
                if (!film) {
                    return next(new Error('Movie not found'));
                }

                const preselectedMovie = {
                    film_id: film.film_id ?? film.filmId,
                    title: film.title,
                    release_year: film.release_year ?? film.releaseYear,
                    rental_rate: film.rental_rate ?? film.rentalRate,
                    rental_duration: film.rental_duration ?? film.rentalDuration,
                    length: film.length,
                };
                const preselectedMovieB64 = Buffer.from(JSON.stringify(preselectedMovie)).toString(
                    'base64'
                );

                // Render the rent movie page
                res.render('staff/rentMovie', {
                    title: 'Rent Movie',
                    preselectedCustomer: null,
                    preselectedMovie,
                    // NEW: safe bootstrap values (no Handlebars in script tags)
                    preselectedCustomerB64: '',
                    preselectedMovieB64,
                    staff: staff || null,
                    user: req.user || null,
                    actionUrl: '/rentals/new',
                    actionMethod: 'POST',
                    returnUrl: '/movies/' + movieId,
                    stores,
                });
            });
        });
    });
});

export default moviesRouter;
