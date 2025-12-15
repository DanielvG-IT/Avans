import { useState } from "react";
import { authApi } from "@/services/api.service";
import { useNavigate } from "react-router-dom";

const Login = () => {
  const navigate = useNavigate();

  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");

  const [SuccessMessage, setSuccessMessage] = useState<string>("");
  const [errorMessage, setErrorMessage] = useState<string>("");

  const handleLogin = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setErrorMessage("");
    setSuccessMessage("");

    if (!email.trim() || !password) {
      setErrorMessage("Please enter both email and password.");
      return;
    }

    try {
      await authApi.login(email, password);

      // Successful response
      setSuccessMessage("Login successful!");
      // Clear sensitive input
      setPassword("");

      setTimeout(() => {
        navigate("/", { replace: true });
      }, 800);
    } catch (err: unknown) {
      console.error("Unexpected login error:", err);
      if (err instanceof Error && err.message) {
        setErrorMessage(err.message);
      } else {
        setErrorMessage(
          "An unexpected error occurred. Please check your connection and try again.",
        );
      }
    }
  };

  return (
    <div className="w-full max-w-md bg-card/95 backdrop-blur-md rounded-2xl shadow-2xl p-8 border border-border">
      <header className="text-center mb-6">
        <h1 className="text-3xl font-semibold text-card-foreground">Welcome</h1>
        <p className="text-sm text-muted-foreground mt-1">
          Sign in to continue to Avans Keuzekompas
        </p>
      </header>

      <form className="space-y-4" aria-label="Login form" onSubmit={handleLogin}>
        <div className="relative">
          <label htmlFor="emailInput" className="text-sm font-medium text-card-foreground">
            Email
          </label>
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none mt-6">
            <svg
              className="w-5 h-5 text-muted-foreground"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              aria-hidden
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                d="M16 12l-4-4-4 4m8 0l-4 4-4-4"
              />
            </svg>
          </div>
          <input
            id="emailInput"
            name="email"
            type="email"
            placeholder="Email address"
            required
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full pl-10 pr-4 py-3 bg-background border border-input text-foreground placeholder:text-muted-foreground rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 transition-colors"
            aria-describedby="email-note"
          />
          <p id="email-note" className="sr-only">
            Enter your email address
          </p>
        </div>
        <div className="relative">
          <label htmlFor="passwordInput" className="text-sm font-medium text-card-foreground">
            Password
          </label>
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none mt-6">
            <svg
              className="w-5 h-5 text-muted-foreground"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              aria-hidden
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                d="M12 11c1.657 0 3-1.343 3-3S13.657 5 12 5s-3 1.343-3 3 1.343 3 3 3z"
              />
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                d="M17 11v6a2 2 0 0 1-2 2H9a2 2 0 0 1-2-2v-6"
              />
            </svg>
          </div>
          <input
            id="passwordInput"
            name="password"
            type="password"
            placeholder="••••••••"
            required
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full pl-10 pr-4 py-3 bg-background border border-input text-foreground placeholder:text-muted-foreground rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 transition-colors"
          />
        </div>

        <div className="space-y-3 mt-2">
          {errorMessage && (
            <div
              role="alert"
              aria-live="assertive"
              className="flex items-start gap-3 bg-destructive/10 border border-destructive/30 text-destructive px-4 py-2 rounded-lg shadow-sm"
            >
              <svg
                className="w-5 h-5 flex-shrink-0"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
                aria-hidden
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth="2"
                  d="M12 9v2m0 4h.01M5.07 19a9 9 0 1113.86 0L12 21l-6.93-2z"
                />
              </svg>
              <div className="text-sm">{errorMessage}</div>
            </div>
          )}

          {SuccessMessage && (
            <div
              role="status"
              aria-live="polite"
              className="flex items-start gap-3 bg-green-500/10 border border-green-500/30 text-green-700 dark:text-green-400 px-4 py-2 rounded-lg shadow-sm"
            >
              <svg
                className="w-5 h-5 flex-shrink-0"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
                aria-hidden
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth="2"
                  d="M5 13l4 4L19 7"
                />
              </svg>
              <div className="text-sm">{SuccessMessage}</div>
            </div>
          )}
        </div>

        <button
          type="submit"
          className="w-full py-3 rounded-lg bg-primary text-primary-foreground font-medium shadow hover:bg-primary/90 hover:scale-[1.01] transform transition-all focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2"
        >
          Sign in
        </button>
      </form>
    </div>
  );
};

export default Login;
