import { describe, it, expect } from 'vitest';
import { render, screen } from '../helpers/test-utils';
import ErrorState from '@/components/ErrorState';

describe('ErrorState', () => {
  it('renders children when no state is active', () => {
    render(
      <ErrorState>
        <div>Normal content</div>
      </ErrorState>
    );

    expect(screen.getByText('Normal content')).toBeInTheDocument();
  });

  describe('Loading State', () => {
    it('shows loading state with default messages', () => {
      render(<ErrorState loading={true} />);

      expect(screen.getByText('Laden…')).toBeInTheDocument();
      expect(screen.getByText('Even geduld aub')).toBeInTheDocument();
    });

    it('shows loading state with custom messages', () => {
      render(
        <ErrorState
          loading={true}
          loadingTitle="Loading data"
          loadingMessage="Fetching information from server"
        />
      );

      expect(screen.getByText('Loading data')).toBeInTheDocument();
      expect(screen.getByText('Fetching information from server')).toBeInTheDocument();
    });

    it('displays spinner in loading state', () => {
      const { container } = render(<ErrorState loading={true} />);
      
      const spinner = container.querySelector('.animate-spin');
      expect(spinner).toBeInTheDocument();
    });
  });

  describe('Error State', () => {
    it('shows error message when error is a string', () => {
      render(<ErrorState error="Something went wrong" />);

      expect(screen.getByText('Something went wrong')).toBeInTheDocument();
    });

    it('shows error message when error is an Error object', () => {
      const error = new Error('Network error occurred');
      render(<ErrorState error={error} />);

      expect(screen.getByText('Network error occurred')).toBeInTheDocument();
    });

    it('displays error icon', () => {
      const { container } = render(<ErrorState error="Error message" />);
      
      const errorIcon = container.querySelector('svg');
      expect(errorIcon).toBeInTheDocument();
    });
  });

  describe('Empty State', () => {
    it('shows empty state when itemsCount is 0', () => {
      render(<ErrorState itemsCount={0} />);

      expect(screen.getByText('Geen electives gevonden')).toBeInTheDocument();
      expect(screen.getByText('Er zijn nog geen electives beschikbaar.')).toBeInTheDocument();
    });

    it('shows empty state with custom messages', () => {
      render(
        <ErrorState
          itemsCount={0}
          emptyTitle="No results"
          emptyMessage="Try adjusting your filters"
        />
      );

      expect(screen.getByText('No results')).toBeInTheDocument();
      expect(screen.getByText('Try adjusting your filters')).toBeInTheDocument();
    });

    it('does not show empty state when itemsCount is greater than 0', () => {
      render(
        <ErrorState itemsCount={5}>
          <div>Content</div>
        </ErrorState>
      );

      expect(screen.queryByText('Geen electives gevonden')).not.toBeInTheDocument();
      expect(screen.getByText('Content')).toBeInTheDocument();
    });
  });

  describe('Priority', () => {
    it('shows loading state over error state', () => {
      render(<ErrorState loading={true} error="Error message" />);

      expect(screen.getByText('Laden…')).toBeInTheDocument();
      expect(screen.queryByText('Error message')).not.toBeInTheDocument();
    });

    it('shows error state over empty state', () => {
      render(<ErrorState error="Error message" itemsCount={0} />);

      expect(screen.getByText('Error message')).toBeInTheDocument();
      expect(screen.queryByText('Geen electives gevonden')).not.toBeInTheDocument();
    });
  });

  it('applies custom className', () => {
    const { container } = render(<ErrorState loading={true} className="custom-error-class" />);

    expect(container.querySelector('.custom-error-class')).toBeInTheDocument();
  });
});
