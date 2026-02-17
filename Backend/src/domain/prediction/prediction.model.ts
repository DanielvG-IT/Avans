/**
 * Represents the payload sent to the AI service for module prediction
 * All array fields must contain at least one element
 */
export interface PredictionPayload {
  /** Current study program (e.g., 'Informatica') */
  current_study: string;
  /** Student interests - at least 1 required */
  interests: string[];
  /** Study credit range [min, max] in EC */
  wanted_study_credit_range: [number, number];
  /** Preferred study locations - at least 1 required */
  location_preference: string[];
  /** Learning goals for the module - at least 1 required */
  learning_goals: string[];
  /** Preferred education level (NLQF) - at least 1 required */
  level_preference: string[];
  /** Preferred teaching language (e.g., 'Nederlands', 'English') */
  preferred_language: string;
  /** Preferred study periods - at least 1 required */
  preferred_period: string[];
}

/**
 * Represents the AI service response containing module predictions
 */
export interface PredictionResponse {
  /** Top 5 matching modules with similarity scores */
  filtered_top_5_matches: PredictionMatch[];
}

/**
 * Represents a single module prediction match from the AI service
 */
export interface PredictionMatch {
  /** Module ID from the database */
  id: number;
  /** Cosine similarity score (0-1) indicating match quality */
  similarity_score: number;
  /** AI-generated motivation explaining why this module matches */
  motivation: string;
}
