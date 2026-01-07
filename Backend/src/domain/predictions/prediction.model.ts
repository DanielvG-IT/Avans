/**
 * Interface representing the payload sent to the AI service
 */
export interface PredictionPayload {
  current_study: string;
  interests: string[];
  wanted_study_credit_range: [number, number];
  location_preference: string[];
  learning_goals: string[];
  level_preference: string[];
  preferred_language: string;
}

/**
 * Interface wrapping the AI service response
 */
export interface PredictionResponse {
  filtered_top_5_matches: PredictionMatch[];
}

/**
 * Interface representing the AI service output
 */
export interface PredictionMatch {
  id: number;
  similarity_score: number;
}
