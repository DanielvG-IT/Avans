import { Link } from "react-router-dom";
import type { User } from "@/types/User";
import keuzekompasLogo from "@/assets/keuzekompas.svg";
import React, { useEffect, useRef, useState } from "react";
import { ThemeToggle } from "@/components/ThemeToggle";

type HeaderProps = {
  user: User | undefined;
  onLogout?: () => void;
};

interface NavItem {
  title: string;
  href: string;
  roles?: string[];
}

const navItems: NavItem[] = [
  { title: "Home", href: "/" },
  { title: "Electives", href: "/electives" },
  { title: "Users", href: "/users", roles: ["admin"] },
  { title: "Recommendations", href: "/recommendations", roles: ["student"] },
];

const Header: React.FC<HeaderProps> = ({ user, onLogout }) => {
  const [mobileOpen, setMobileOpen] = useState(false);
  const [profileOpen, setProfileOpen] = useState(false);
  const profileRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    function onOutside(e: MouseEvent) {
      if (profileRef.current && !profileRef.current.contains(e.target as Node)) {
        setProfileOpen(false);
      }
    }
    document.addEventListener("mousedown", onOutside);
    return () => document.removeEventListener("mousedown", onOutside);
  }, []);

  const initials = user ? user.firstName[0].toUpperCase() + user.lastName[0].toUpperCase() : "";

  return (
    <header className="bg-background border-b border-border shadow-sm sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-3 sm:px-4 md:px-6 lg:px-8">
        <div className="flex items-center justify-between h-14 sm:h-16">
          {/* Left: Logo */}
          <Link
            to="/"
            className="flex items-center gap-2 sm:gap-3 text-foreground no-underline hover:opacity-80 transition-opacity min-w-0 flex-shrink"
            aria-label="Homepage"
          >
            {keuzekompasLogo && (
              <img
                src={keuzekompasLogo}
                alt="Keuzekompas Logo"
                className="h-6 sm:h-8 w-auto flex-shrink-0"
              />
            )}
            <span className="font-semibold text-sm sm:text-base lg:text-lg truncate">
              Keuzekompas
            </span>
          </Link>

          {/* Center: Nav (desktop/tablet) */}
          <nav className="hidden md:flex items-center space-x-4 lg:space-x-6">
            {navItems.map((item) => {
              if (!item.roles || (user && item.roles.includes(user.role))) {
                return (
                  <Link
                    key={item.title}
                    to={item.href}
                    className="text-sm lg:text-base text-muted-foreground hover:text-foreground transition-colors whitespace-nowrap px-2 py-2"
                  >
                    {item.title}
                  </Link>
                );
              }
            })}
          </nav>

          {/* Right: Auth / Theme Toggle / Mobile toggle */}
          <div className="flex items-center gap-1 sm:gap-2 md:gap-3">
            {/* Theme Toggle - visible on all screen sizes */}
            <div className="flex-shrink-0">
              <ThemeToggle />
            </div>

            <div className="hidden md:block">
              {user ? (
                <div className="relative" ref={profileRef}>
                  <button
                    onClick={() => setProfileOpen((s) => !s)}
                    className="flex items-center gap-2 px-2 lg:px-3 py-1.5 rounded-full hover:bg-accent hover:text-accent-foreground focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 transition-colors"
                    aria-haspopup="true"
                    aria-expanded={profileOpen}
                  >
                    <span className="w-8 h-8 lg:w-9 lg:h-9 rounded-full bg-primary text-primary-foreground flex items-center justify-center text-xs lg:text-sm font-medium flex-shrink-0">
                      {initials}
                    </span>
                    <span className="text-sm text-foreground hidden lg:inline">
                      {user?.firstName}
                    </span>
                  </button>

                  {profileOpen && (
                    <div className="absolute right-0 mt-2 w-44 bg-popover border border-border rounded-md shadow-lg py-1 z-10">
                      <Link
                        to="/profile"
                        className="block px-4 py-2 text-sm text-popover-foreground hover:bg-accent hover:text-accent-foreground transition-colors"
                      >
                        Profile
                      </Link>
                      <button
                        onClick={() => {
                          setProfileOpen(false);
                          onLogout?.();
                        }}
                        className="w-full text-left px-4 py-2 text-sm text-popover-foreground hover:bg-accent hover:text-accent-foreground transition-colors"
                      >
                        Logout
                      </button>
                    </div>
                  )}
                </div>
              ) : null}
            </div>

            {/* Mobile menu button */}
            <button
              className="md:hidden inline-flex items-center justify-center p-2 rounded-md text-muted-foreground hover:bg-accent hover:text-accent-foreground focus:ring-2 focus:ring-ring focus:ring-offset-2 transition-colors min-w-touch min-h-touch"
              onClick={() => setMobileOpen((s) => !s)}
              aria-expanded={mobileOpen}
              aria-label="Toggle menu"
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                {mobileOpen ? (
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M6 18L18 6M6 6l12 12"
                  />
                ) : (
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M4 6h16M4 12h16M4 18h16"
                  />
                )}
              </svg>
            </button>
          </div>
        </div>
      </div>

      {/* Mobile panel */}
      {mobileOpen && (
        <div className="md:hidden border-t border-border bg-background">
          <div className="px-3 sm:px-4 pt-3 pb-4 space-y-1 max-h-[calc(100vh-4rem)] overflow-y-auto">
            {navItems.map((item) => {
              if (!item.roles || (user && item.roles.includes(user.role))) {
                return (
                  <Link
                    key={item.title}
                    to={item.href}
                    onClick={() => setMobileOpen(false)}
                    className="block px-3 py-3 text-base font-medium text-foreground rounded-lg hover:bg-accent hover:text-accent-foreground transition-colors min-h-touch"
                  >
                    {item.title}
                  </Link>
                );
              }
            })}

            {user ? (
              <div className="pt-3 border-t border-border mt-3">
                <div className="flex items-center gap-3 px-3 py-2">
                  <div className="w-10 h-10 rounded-full bg-primary text-primary-foreground flex items-center justify-center font-medium text-sm">
                    {initials}
                  </div>
                  <div className="min-w-0 flex-1">
                    <div className="text-sm font-medium text-foreground truncate">
                      {user?.firstName} {user?.lastName}
                    </div>
                    <div className="text-xs text-muted-foreground truncate">{user?.email}</div>
                  </div>
                </div>
                <Link
                  to="/profile"
                  onClick={() => setMobileOpen(false)}
                  className="block mx-3 mt-2 px-3 py-2 rounded-md text-sm text-foreground hover:bg-accent hover:text-accent-foreground transition-colors min-h-touch"
                >
                  View profile
                </Link>
                <div className="px-3 pb-2 pt-3">
                  <button
                    onClick={() => {
                      setMobileOpen(false);
                      onLogout?.();
                    }}
                    className="w-full text-left px-3 py-3 rounded-lg bg-destructive/10 text-destructive text-sm font-medium hover:bg-destructive/20 transition-colors focus:ring-2 focus:ring-ring focus:ring-offset-2 min-h-touch"
                  >
                    Logout
                  </button>
                </div>
              </div>
            ) : null}
          </div>
        </div>
      )}
    </header>
  );
};

export default Header;
